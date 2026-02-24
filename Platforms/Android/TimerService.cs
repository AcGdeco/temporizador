using global::Android.App;
using global::Android.Content;
using global::Android.OS;
using global::Android.Media;
using global::Android.Content.Res;
using AndroidX.Core.App;
using Temporizador.Platforms.Android;
using CommunityToolkit.Mvvm.Messaging;
using CoreNotificationCompat = AndroidX.Core.App.NotificationCompat;
using System;
using Microsoft.Maui.Storage;
using Temporizador; // Para mensagens

namespace Temporizador.Platforms.Android
{
    [Service(Exported = true, ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeSpecialUse)]
    public class TimerService : Service
    {
        private const int NotificationId = 1;
        private const string ChannelId = "timer_channel";

        // Ações simplificadas
        public const string ActionPausar = "PAUSAR";
        public const string ActionIniciar = "INICIAR";
        public const string ActionParar = "PARAR";
        public const string ActionResetar = "RESETAR";
        public const string ActionUpdate = "UPDATE";
        public const string ActionExpirar = "EXPIRAR";

        private NotificationManager _notificationManager;
        private bool _isForeground = false;

        // Estado do timer
        private DateTime _targetEndTime;
        private TimeSpan _tempoRestante;
        private TimeSpan _tempoInicial;
        private bool _estaRodando = false;

        // AlarmManager para notificações
        private AlarmManager _alarmManager;
        private PendingIntent _updateIntent;
        private PendingIntent _expireIntent;

        // Throttling para atualizações
        private const long MinuteUpdateIntervalMs = 60 * 1000;

        // Screen / minute update handling
        private PowerManager _powerManager;
        private BroadcastReceiver _screenReceiver;

        // Handler para posts atrasados
        private Handler _handler;
        private MediaPlayer? _alarmPlayer;
        private Vibrator? _vibrator;
        private bool _alarmeAtivo;

        // Método estático para obter ação baseada no texto do botão
        public static string GetActionForButton(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return ActionParar;

            return texto.ToUpperInvariant() switch
            {
                "PAUSAR" => ActionPausar,
                "INICIAR" => ActionIniciar,
                "CONTINUAR" => ActionIniciar,
                "PARAR" => ActionParar,
                "RESET" => ActionResetar,
                _ => ActionUpdate
            };
        }

        public override IBinder? OnBind(Intent? intent) => null;

        private void SaveState()
        {
            try
            {
                Preferences.Default.Set("estaRodando", _estaRodando);
                if (_estaRodando)
                {
                    Preferences.Default.Set("targetEndTimeTicks", _targetEndTime.Ticks);
                }
                else
                {
                    Preferences.Default.Set("tempoRestanteTicks", _tempoRestante.Ticks);
                }
                Preferences.Default.Set("tempoInicialTicks", _tempoInicial.Ticks);
            }
            catch { }
        }

        private void LoadState()
        {
            try
            {
                _estaRodando = Preferences.Default.Get("estaRodando", false);
                _tempoInicial = TimeSpan.FromTicks(Preferences.Default.Get("tempoInicialTicks", 0L));

                if (_estaRodando)
                {
                    long targetTicks = Preferences.Default.Get("targetEndTimeTicks", 0L);
                    if (targetTicks > 0)
                    {
                        _targetEndTime = new DateTime(targetTicks, DateTimeKind.Utc);
                        // Verifica se já expirou
                        if (DateTime.UtcNow >= _targetEndTime)
                        {
                            _estaRodando = false;
                            _tempoRestante = TimeSpan.Zero;
                        }
                        else
                        {
                            _tempoRestante = _targetEndTime - DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        _estaRodando = false;
                    }
                }
                
                if (!_estaRodando)
                {
                    _tempoRestante = TimeSpan.FromTicks(Preferences.Default.Get("tempoRestanteTicks", 0L));
                }
            }
            catch { }
        }

        // Força atualização imediata quando a tela é acesa
        private void ForceUpdateNotificationOnScreenOn()
        {
            if (!_estaRodando) return;
            // atualiza imediatamente e garante que o próximo agendamento esteja alinhado por minuto
            _handler?.Post(() => { UpdateNotification(force: true); ScheduleNextUpdate(); });
        }

        // Receiver para mudanças no estado da tela
        private class ScreenStateReceiver : BroadcastReceiver
        {
            private readonly TimerService _svc;
            public ScreenStateReceiver(TimerService svc) => _svc = svc;
            public override void OnReceive(Context? context, Intent? intent)
            {
                if (intent?.Action == Intent.ActionScreenOn || intent?.Action == Intent.ActionUserPresent)
                {
                    _svc.ForceUpdateNotificationOnScreenOn();
                }
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
            _alarmManager = (AlarmManager)GetSystemService(Context.AlarmService)!;
            _handler = new Handler(Looper.MainLooper!);
            _vibrator = (Vibrator)GetSystemService(VibratorService)!;

            // Inicializa PowerManager e registra receiver para atualização ao acender a tela
            _powerManager = (PowerManager)GetSystemService(Context.PowerService)!;
            _screenReceiver = new ScreenStateReceiver(this);
            var screenFilter = new IntentFilter();
            screenFilter.AddAction(Intent.ActionScreenOn);
            screenFilter.AddAction(Intent.ActionUserPresent);
            RegisterReceiver(_screenReceiver, screenFilter);

            // Cria o canal de notificação
            CreateNotificationChannel();

            // Prepara os PendingIntents
            var updateIntent = new Intent(this, typeof(TimerService)).SetAction(ActionUpdate);
            _updateIntent = PendingIntent.GetService(this, 1, updateIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
            
            var expireIntent = new Intent(this, typeof(TimerService)).SetAction(ActionExpirar);
            _expireIntent = PendingIntent.GetService(this, 2, expireIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Canal do Temporizador", 
                    NotificationImportance.Low)
                {
                    Description = "Notificações do temporizador",
                    LockscreenVisibility = NotificationVisibility.Public
                };
                
                // Configurações separadas (não no inicializador)
                channel.EnableVibration(false);
                channel.EnableLights(false);
                channel.SetSound(null, null);
                
                _notificationManager.CreateNotificationChannel(channel);
            }
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            // Log de depuração para verificar toques nas ações da notificação
            try {
                global::Android.Util.Log.Debug("TimerService", $"OnStartCommand: rawAction={intent?.Action}, tempoExtra={intent?.GetStringExtra("tempo")}");
            } catch { }

            // Normaliza ações que podem vir com prefixo vindo da notificação (ex.: "ACAO_PAUSAR")
            string? rawAction = intent?.Action;
            string? action = rawAction;
            if (!string.IsNullOrEmpty(rawAction) && rawAction.StartsWith("ACAO_"))
                action = rawAction.Substring("ACAO_".Length);

            if (!string.IsNullOrEmpty(action))
            {
                switch (action)
                {
                    case ActionUpdate:
                        HandleUpdate();
                        break;
                    case ActionPausar:
                        PausarTimer();
                        break;
                    case ActionIniciar:
                        RetomarTimer(intent?.GetStringExtra("tempo"));
                        break;
                    case ActionParar:
                        PararTimer();
                        break;
                    case ActionResetar:
                        ResetarTimer();
                        break;
                    case ActionExpirar:
                        ExpirarTimer();
                        break;
                }
            }
            else
            {
                // Inicialização ou restart
                if (intent != null && (intent.HasExtra("tempo") || intent.HasExtra("estaRodando")))
                {
                    // Inicialização explícita
                    var tempoStr = intent.GetStringExtra("tempo") ?? "00:00:00";
                    var tempoInicialStr = intent.GetStringExtra("tempoInicial") ?? tempoStr;
                    var rodando = intent.GetBooleanExtra("estaRodando", false);

                    TimeSpan.TryParse(tempoStr, out _tempoRestante);
                    TimeSpan.TryParse(tempoInicialStr, out _tempoInicial);
                    _estaRodando = rodando;

                    if (_estaRodando && _tempoRestante.TotalSeconds > 0)
                    {
                        StartTimerLogic();
                    }
                    else
                    {
                         // Se não estiver rodando, apenas salva e atualiza notificação
                         SaveState();
                    }
                }
                else
                {
                    // Restart pelo sistema - recupera estado
                    LoadState();
                    if (_estaRodando && _targetEndTime > DateTime.UtcNow)
                    {
                        // Recalcula tempo restante e reinicia lógica
                        _tempoRestante = _targetEndTime - DateTime.UtcNow;
                        ScheduleExpireAlarm();
                        ScheduleNextUpdate();
                    }
                    else if (_estaRodando) // Já expirou enquanto estava morto
                    {
                        _estaRodando = false;
                        _tempoRestante = TimeSpan.Zero;
                        ExpirarTimer();
                    }
                }
            }

            // Atualiza a notificação (apenas se necessário)
            UpdateNotification();

            // Inicia o serviço em foreground (se ainda não estiver)
            if (!_isForeground)
            {
                var notification = BuildNotification();
                if (Build.VERSION.SdkInt >= BuildVersionCodes.UpsideDownCake)
                {
                     // Android 14 exige tipo de serviço
                     StartForeground(NotificationId, notification, global::Android.Content.PM.ForegroundService.TypeSpecialUse);
                }
                else
                {
                    StartForeground(NotificationId, notification);
                }
                _isForeground = true;
            }

            return StartCommandResult.Sticky;
        }

        private void StartTimerLogic()
        {
            _targetEndTime = DateTime.UtcNow.Add(_tempoRestante);
            _estaRodando = true;
            SaveState();

            ScheduleExpireAlarm();
            ScheduleNextUpdate();
        }

        private void PausarTimer()
        {
            if (!_estaRodando) return;

            _estaRodando = false;
            // Recalcula o tempo restante baseado no tempo alvo
            var remaining = _targetEndTime - DateTime.UtcNow;
            _tempoRestante = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            
            SaveState();
            
            CancelAlarms();
            _handler.RemoveCallbacksAndMessages(null);
            
            UpdateNotification();
            
            // Envia mensagem para a UI atualizar (sem loop)
            WeakReferenceMessenger.Default.Send(new PausarTimerPelaNotificacaoMessage());
        }

        private void RetomarTimer(string? tempoStr)
        {
             // Se veio tempo na intent, usa, senão usa o estado atual
             if (!string.IsNullOrEmpty(tempoStr) && TimeSpan.TryParse(tempoStr, out var t))
             {
                 _tempoRestante = t;
             }
             
             if (_tempoRestante.TotalSeconds <= 0) return;

             StartTimerLogic();
             
             // Envia mensagem para a UI atualizar
             WeakReferenceMessenger.Default.Send(new IniciarTimerPelaNotificacaoMessage());
        }

        private void PararTimer()
        {
            _estaRodando = false;
            _tempoRestante = _tempoInicial; // Volta ao tempo inicial
            SaveState();
            
            CancelAlarms();
            _handler.RemoveCallbacksAndMessages(null);
            PararAlarmeEVibracao();
            
            UpdateNotification();
            
            WeakReferenceMessenger.Default.Send(new PararTimerPelaNotificacaoMessage());
        }

        private void ResetarTimer()
        {
            _estaRodando = false;
            _tempoRestante = TimeSpan.Zero;
            _tempoInicial = TimeSpan.Zero;
            SaveState();
            
            CancelAlarms();
            _handler.RemoveCallbacksAndMessages(null);
            PararAlarmeEVibracao();
            
            StopForeground(true);
            _notificationManager.Cancel(NotificationId);
            _isForeground = false;
            StopSelf();
            
            WeakReferenceMessenger.Default.Send(new ResetarTimerPelaNotificacaoMessage());
        }

        private void CancelAlarms()
        {
            try
            {
                _alarmManager?.Cancel(_updateIntent);
                _alarmManager?.Cancel(_expireIntent);
            }
            catch {}
        }

        private void ScheduleExpireAlarm()
        {
            try
            {
                long endTimeMillis = SystemClock.ElapsedRealtime() + (long)(_targetEndTime - DateTime.UtcNow).TotalMilliseconds;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                {
                    _alarmManager.Set(AlarmType.ElapsedRealtime, endTimeMillis, _expireIntent);
                }
                else
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                        _alarmManager.SetExactAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, endTimeMillis, _expireIntent);
                    else
                        _alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, endTimeMillis, _expireIntent);
                }
            }
            catch
            {
                // Fallback: usa Handler
                ScheduleExpireWithHandler();
            }
        }

        private void ScheduleExpireWithHandler()
        {
            long remainingMs = (long)(_targetEndTime - DateTime.UtcNow).TotalMilliseconds;
            _handler.PostDelayed(() => {
                if (_estaRodando && DateTime.UtcNow >= _targetEndTime)
                {
                    ExpirarTimer();
                }
                else if (_estaRodando)
                {
                    ScheduleExpireWithHandler();
                }
            }, Math.Max(100, remainingMs));
        }

        private void ScheduleNextUpdate()
        {
            _alarmManager?.Cancel(_updateIntent);

            long now = SystemClock.ElapsedRealtime();
            long remainingMs = (long)(_targetEndTime - DateTime.UtcNow).TotalMilliseconds;
            
            if (remainingMs <= 0) return;

            try
            {
                if (remainingMs > MinuteUpdateIntervalMs)
                {
                    long scheduleAt = now + Math.Min(remainingMs - 1000, MinuteUpdateIntervalMs);
                    
                     if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                        _alarmManager.Set(AlarmType.ElapsedRealtime, scheduleAt, _updateIntent);
                    else
                    {
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                            _alarmManager.SetExactAndAllowWhileIdle(AlarmType.ElapsedRealtimeWakeup, scheduleAt, _updateIntent);
                        else
                            _alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, scheduleAt, _updateIntent);
                    }
                }
            }
            catch
            {
            }
        }

        private void HandleUpdate()
        {
            if (!_estaRodando) return;

            // 1. Calcula a diferença real
            double totalMilliseconds = (_targetEndTime - DateTime.UtcNow).TotalMilliseconds;

            if (totalMilliseconds <= 0)
            {
                ExpirarTimer();
                return;
            }

            // 2. Arredonda para cima os segundos para a UI não parecer "atrasada"
            // Exemplo: 1500ms -> 1.5s -> Ceiling vira 2s -> 2000ms
            double segundosArredondados = Math.Ceiling(totalMilliseconds / 1000.0);
            long remainingArredondado = (long)(segundosArredondados * 1000);

            _tempoRestante = TimeSpan.FromMilliseconds(remainingArredondado);

            // Salva estado periodicamente para caso de crash
            SaveState();

            UpdateNotification();
            ScheduleNextUpdate();
        }

        private void ExpirarTimer()
        {
            _estaRodando = false;
            _tempoRestante = TimeSpan.Zero;
            SaveState();
            IniciarAlarmeEVibracao();
            UpdateNotification(true);
        }

        private void IniciarAlarmeEVibracao()
        {
            if (_alarmeAtivo) return;
            try
            {
                _alarmPlayer = new MediaPlayer();
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var attrs = new AudioAttributes.Builder()
                        .SetUsage(AudioUsageKind.Alarm)
                        .SetContentType(AudioContentType.Sonification)
                        .Build();
                    _alarmPlayer.SetAudioAttributes(attrs);
                }
                _alarmPlayer.Looping = true;
                _alarmPlayer.SetVolume(1f, 1f);
                _alarmPlayer.Prepare();
                _alarmPlayer.Start();
            }
            catch { }
            try
            {
                long[] pattern = new long[] { 0, 1000, 1000 };
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var effect = VibrationEffect.CreateWaveform(pattern, 0);
                    _vibrator?.Vibrate(effect);
                }
                else
                {
                    _vibrator?.Vibrate(pattern, 0);
                }
            }
            catch { }
            _alarmeAtivo = true;
        }

        private void PararAlarmeEVibracao()
        {
            try
            {
                _alarmPlayer?.Stop();
                _alarmPlayer?.Release();
                _alarmPlayer = null;
            }
            catch { }
            try
            {
                _vibrator?.Cancel();
            }
            catch { }
            _alarmeAtivo = false;
        }

        private void UpdateNotification(bool force = false)
        {
            if (!_isForeground) return;
            var notification = BuildNotification();
            _notificationManager.Notify(NotificationId, notification);
        }

        private Notification BuildNotification()
        {
            string contentText;
            long when = 0;
            bool useChronometer = false;

            // Arredonda SEMPRE para cima. Ex: 59.1 segundos viram 60 segundos (1 minuto).
            double segundosTotaisArredondados = Math.Ceiling(_tempoRestante.TotalSeconds);
            TimeSpan tempoArredondado = TimeSpan.FromSeconds(segundosTotaisArredondados);

            if (_estaRodando)
            {
                // O Chronometer do Android usa o timestamp de destino. 
                // Ele arredonda internamente, então mantemos o target real.
                when = new DateTimeOffset(_targetEndTime).ToUnixTimeMilliseconds() + 1000;
                useChronometer = true;
                
                if (tempoArredondado.TotalSeconds >= 60)
                {
                    // Usamos TotalMinutes arredondado para cima para o texto auxiliar
                    int totalMinutes = (int)Math.Ceiling(tempoArredondado.TotalMinutes);
                    
                    if (totalMinutes >= 60)
                    {
                        int h = totalMinutes / 60;
                        int m = totalMinutes % 60;
                        contentText = m > 0 ? $"Termina em {h}h {m}m" : $"Termina em {h}h";
                    }
                    else
                    {
                        contentText = $"Termina em {totalMinutes} min";
                    }
                }
                else if (tempoArredondado.TotalSeconds > 0)
                {
                    contentText = "Quase pronto!";
                }
                else
                {
                    contentText = "Tempo esgotado!";
                }
            }
            else
            {
                // Estado Pausado: Mostra o tempo estático arredondado
                contentText = FormatTimeForNotification(tempoArredondado);
            }

            string textoBotao1 = _estaRodando ? "Pausar" : "Iniciar";
            string textoBotao2 = _estaRodando ? "Parar" : "Reset";

            // Enviamos a string formatada com o tempo arredondado para os botões/broadcasts
            var builder = TimerNotificationBuilder.Build(
                this, 
                textoBotao1, 
                textoBotao2, 
                tempoArredondado.ToString(@"hh\:mm\:ss"), 
                when, 
                useChronometer);
                
            return builder
                .SetContentTitle(_estaRodando ? "Cozinhando..." : "Timer Pausado")
                .SetContentText(contentText)
                .SetOngoing(_estaRodando)
                .SetOnlyAlertOnce(true)
                .Build();
        }

        private string FormatTimeForNotification(TimeSpan time)
        {
            return time.ToString(@"hh\:mm\:ss");
        }
    }
}
