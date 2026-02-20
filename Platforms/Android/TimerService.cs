using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;
using Temporizador.Platforms.Android;
using CommunityToolkit.Mvvm.Messaging;
using CoreNotificationCompat = AndroidX.Core.App.NotificationCompat;
using System;

namespace Temporizador
{
    [Service(Exported = true, ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
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

        private NotificationManager _notificationManager;
        private bool _isForeground = false;

        // Estado do timer
        private long _endTimeMillis;
        private TimeSpan _tempoRestante;
        private TimeSpan _tempoInicial;
        private bool _estaRodando = false;

        // AlarmManager para notificações
        private AlarmManager _alarmManager;
        private PendingIntent _updateIntent;
        private PendingIntent _expireIntent;

        // Throttling para atualizações
        private long _lastUpdateTime = 0;
        private const long MinUpdateIntervalMs = 1000;

        // Screen / minute update handling
        private PowerManager _powerManager;
        private BroadcastReceiver _screenReceiver;
        private int _lastNotifiedMinutes = -1;
        private const long MinuteUpdateIntervalMs = 60 * 1000;

        // Handler para posts atrasados
        private Handler _handler;
        private const long HandlerUpdateIntervalMs = 1000;

        // Método estático para obter ação baseada no texto do botão
        public static string GetActionForButton(string texto)
        {
            return texto switch
            {
                "Pausar" => ActionPausar,
                "Iniciar" or "Continuar" => ActionIniciar,
                "Parar" => ActionParar,
                "Reset" => ActionResetar,
                _ => ActionUpdate
            };
        }

        public override IBinder OnBind(Intent intent) => null;

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
            public override void OnReceive(Context context, Intent intent)
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

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);
            _alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);
            _handler = new Handler(Looper.MainLooper);

            // Inicializa PowerManager e registra receiver para atualização ao acender a tela
            _powerManager = (PowerManager)GetSystemService(Context.PowerService);
            _screenReceiver = new ScreenStateReceiver(this);
            var screenFilter = new IntentFilter();
            screenFilter.AddAction(Intent.ActionScreenOn);
            screenFilter.AddAction(Intent.ActionUserPresent);
            RegisterReceiver(_screenReceiver, screenFilter);

            // Cria o canal de notificação
            CreateNotificationChannel();

            // Prepara os PendingIntents
            _updateIntent = PendingIntent.GetService(this, 1, 
                new Intent(this, typeof(TimerService)).SetAction(ActionUpdate), 
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            
            _expireIntent = PendingIntent.GetService(this, 2, 
                new Intent(this, typeof(TimerService)).SetAction(ActionParar), 
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
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

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Log de depuração para verificar toques nas ações da notificação
            try {
                Android.Util.Log.Debug("TimerService", $"OnStartCommand: rawAction={intent?.Action}, tempoExtra={intent?.GetStringExtra("tempo")}");
            } catch { }

            // Normaliza ações que podem vir com prefixo vindo da notificação (ex.: "ACAO_PAUSAR")
            string rawAction = intent?.Action;
            string action = rawAction;
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
                        RetomarTimer(intent.GetStringExtra("tempo"));
                        break;
                    case ActionParar:
                        PararTimer();
                        break;
                    case ActionResetar:
                        ResetarTimer();
                        break;
                }
            }
            else if (intent != null)
            {
                // Inicialização normal
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
            }

            // Atualiza a notificação (apenas se necessário)
            UpdateNotification();

            // Inicia o serviço em foreground (se ainda não estiver)
            if (!_isForeground)
            {
                var notification = BuildNotification();
                StartForeground(NotificationId, notification);
                _isForeground = true;
            }

            return StartCommandResult.Sticky;
        }

        private void StartTimerLogic()
        {
            _endTimeMillis = SystemClock.ElapsedRealtime() + (long)_tempoRestante.TotalMilliseconds;
            _estaRodando = true;

            // força atualização de minuto na próxima execução
            _lastNotifiedMinutes = -1;

            ScheduleExpireAlarm();
            ScheduleNextUpdate();
        }

        private void ScheduleExpireAlarm()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                {
                    _alarmManager.Set(AlarmType.ElapsedRealtime, _endTimeMillis, _expireIntent);
                }
                else
                {
                    _alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, _endTimeMillis, _expireIntent);
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
            _handler.PostDelayed(() => {
                if (_estaRodando && SystemClock.ElapsedRealtime() >= _endTimeMillis)
                {
                    PararTimer();
                }
                else if (_estaRodando)
                {
                    ScheduleExpireWithHandler();
                }
            }, Math.Max(100, _endTimeMillis - SystemClock.ElapsedRealtime()));
        }

        private void ScheduleNextUpdate()
        {
            // Cancela alarme anterior
            _alarmManager?.Cancel(_updateIntent);

            long now = SystemClock.ElapsedRealtime();
            long remainingMs = _endTimeMillis - now;

            if (remainingMs <= 0) return;

            try
            {
                if (remainingMs >= MinuteUpdateIntervalMs)
                {
                    // Usa aritmética inteira para determinar o "display minute" e evitar erros de ponto flutuante
                    long remMs = remainingMs;
                    int displayMinutes = (int)((remMs + MinuteUpdateIntervalMs - 1) / MinuteUpdateIntervalMs);
                    long targetRemaining = (displayMinutes - 1) * MinuteUpdateIntervalMs; // quando o rótulo vai decrementar
                    long delayToNextChange = remainingMs - targetRemaining;
                    long scheduleAt = now + Math.Max(100, delayToNextChange);
                    if (scheduleAt > _endTimeMillis) scheduleAt = _endTimeMillis;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                        _alarmManager.Set(AlarmType.ElapsedRealtime, scheduleAt, _updateIntent);
                    else
                        _alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, scheduleAt, _updateIntent);
                }
                else
                {
                    // Menos de 1 minuto: atualiza por segundo (a notificação só será mostrada se a tela estiver acesa)
                    long nextUpdate = now + 1000 - (now % 1000);
                    if (nextUpdate >= _endTimeMillis) nextUpdate = _endTimeMillis;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                        _alarmManager.Set(AlarmType.ElapsedRealtime, nextUpdate, _updateIntent);
                    else
                        _alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, nextUpdate, _updateIntent);
                }
            }
            catch
            {
                // Fallback: usa Handler
                ScheduleUpdateWithHandler();
            }
        }

        private void ScheduleUpdateWithHandler()
        {
            _handler.RemoveCallbacksAndMessages(null);

            long now = SystemClock.ElapsedRealtime();
            long remainingMs = _endTimeMillis - now;
            long delayMs = HandlerUpdateIntervalMs;

            if (remainingMs >= MinuteUpdateIntervalMs)
            {
                long remMs = remainingMs;
                int displayMinutes = (int)((remMs + MinuteUpdateIntervalMs - 1) / MinuteUpdateIntervalMs);
                long targetRemaining = (displayMinutes - 1) * MinuteUpdateIntervalMs;
                delayMs = Math.Max(100, remainingMs - targetRemaining);
            }

            _handler.PostDelayed(() => {
                if (_estaRodando)
                {
                    HandleUpdate();
                    ScheduleUpdateWithHandler();
                }
            }, delayMs);
        }

        private void HandleUpdate()
        {
            if (!_estaRodando) return;

            long now = SystemClock.ElapsedRealtime();
            long remaining = _endTimeMillis - now;

            // Throttling manual para evitar chamadas excessivas
            if (now - _lastUpdateTime < MinUpdateIntervalMs && remaining > 1000)
                return;

            _lastUpdateTime = now;

            if (remaining <= 0)
            {
                PararTimer();
                return;
            }

            _tempoRestante = TimeSpan.FromMilliseconds(remaining);

            // Atualiza a notificação — UpdateNotification agora decide se deve realmente notificar (tela ligada / mudança de minuto)
            UpdateNotification();

            // Agenda próxima atualização (minuto ou segundo conforme o caso)
            ScheduleNextUpdate();
        }

        private void UpdateNotification(bool force = false)
        {
            if (!_isForeground) return;

            // Só notifica se a tela estiver acesa (a menos que for forçado)
            bool screenOn = _powerManager?.IsInteractive ?? true;
            if (!screenOn && !force) return;

            // Quando há 1 minuto ou mais restantes, atualiza apenas na mudança de "display minute" (usamos ceil)
            if (_tempoRestante.TotalSeconds >= 60)
            {
                long remMs = (long)Math.Max(0, _tempoRestante.TotalMilliseconds);
                int displayMinutes = (int)((remMs + MinuteUpdateIntervalMs - 1) / MinuteUpdateIntervalMs);
                if (!force && displayMinutes == _lastNotifiedMinutes)
                    return; // sem mudança de minuto exibido — ignora
                _lastNotifiedMinutes = displayMinutes;
            }
            else
            {
                // abaixo de 1 minuto, reseta o rastreador de minutos para que próximas mudanças de minuto sejam detectadas
                _lastNotifiedMinutes = -1;
            }

            var notification = BuildNotification();
            _notificationManager.Notify(NotificationId, notification);
        }

        private Notification BuildNotification()
        {
            string contentText;
            if (_tempoRestante.TotalSeconds >= 60)
            {
                long remMs = (long)Math.Max(0, _tempoRestante.TotalMilliseconds);
                int displayMinutes = (int)((remMs + MinuteUpdateIntervalMs - 1) / MinuteUpdateIntervalMs);
                if (displayMinutes >= 60)
                {
                    int hours = displayMinutes / 60;
                    int mins = displayMinutes % 60;
                    contentText = $"Faltando menos que {hours}h : {mins}m";
                }
                else
                {
                    contentText = $"Faltando menos que {displayMinutes} m";
                }
            }
            else
            {
                contentText = FormatTimeForNotification(_tempoRestante);
            }

            string textoBotao1 = _estaRodando ? "Pausar" : "Iniciar";
            string textoBotao2 = _estaRodando ? "Parar" : "Reset";

            // Passamos o tempo restante real como extra (hh:mm:ss) para ações
            return TimerNotificationBuilder.Build(this, textoBotao1, textoBotao2, _tempoRestante.ToString(@"hh\:mm\:ss"))
                .SetContentTitle(_estaRodando ? "Temporizador em andamento" : "Temporizador parado")
                .SetContentText(contentText)
                .SetOngoing(_estaRodando)
                .SetOnlyAlertOnce(true)
                .Build();
        }

        private string FormatTimeForNotification(TimeSpan time)
        {
            int totalSeconds = (int)time.TotalSeconds;
            if (totalSeconds <= 0) return "00:00";

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            if (hours > 0)
                return $"{hours:00}:{minutes:00}:{seconds:00}";
            else
                return $"{minutes:00}:{seconds:00}";
        }

        private void PausarTimer()
        {
            if (!_estaRodando) return;

            _estaRodando = false;
            _tempoRestante = TimeSpan.FromMilliseconds(Math.Max(0, _endTimeMillis - SystemClock.ElapsedRealtime()));
            
            CancelAlarms();
            _handler.RemoveCallbacksAndMessages(null);
            
            UpdateNotification();
            
            WeakReferenceMessenger.Default.Send(new PausarTimerPelaNotificacaoMessage());
        }

        private void RetomarTimer(string tempoStr)
        {
            if (_estaRodando) return;
            
            if (!string.IsNullOrEmpty(tempoStr))
                TimeSpan.TryParse(tempoStr, out _tempoRestante);

            if (_tempoRestante.TotalSeconds > 0)
            {
                StartTimerLogic();
                UpdateNotification();
                
                WeakReferenceMessenger.Default.Send(new IniciarTimerPelaNotificacaoMessage());
            }
        }

        private void PararTimer()
        {
            _estaRodando = false;
            _tempoRestante = _tempoInicial;
            
            CancelAlarms();
            _handler.RemoveCallbacksAndMessages(null);
            
            UpdateNotification(force: true);
            
            WeakReferenceMessenger.Default.Send(new PararTimerPelaNotificacaoMessage());
            
            // Para de executar em foreground (opcional)
            StopForeground(StopForegroundFlags.Detach);
            _isForeground = false;
        }

        private void ResetarTimer()
        {
            _estaRodando = false;
            _tempoRestante = TimeSpan.Zero;
            _tempoInicial = TimeSpan.Zero;
            
            CancelAlarms();
            _handler.RemoveCallbacksAndMessages(null);
            
            _notificationManager.Cancel(NotificationId);
            
            StopForeground(StopForegroundFlags.Remove);
            _isForeground = false;
            
            WeakReferenceMessenger.Default.Send(new ResetarTimerPelaNotificacaoMessage());
        }

        private void CancelAlarms()
        {
            _alarmManager?.Cancel(_updateIntent);
            _alarmManager?.Cancel(_expireIntent);
        }

        public override void OnDestroy()
        {
            CancelAlarms();
            _handler.RemoveCallbacksAndMessages(null);

            if (_screenReceiver != null)
            {
                try { UnregisterReceiver(_screenReceiver); } catch { }
                _screenReceiver = null;
            }

            WeakReferenceMessenger.Default.Unregister<PausarTimerPelaNotificacaoMessage>(this);
            WeakReferenceMessenger.Default.Unregister<IniciarTimerPelaNotificacaoMessage>(this);
            WeakReferenceMessenger.Default.Unregister<PararTimerPelaNotificacaoMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ResetarTimerPelaNotificacaoMessage>(this);
            
            StopForeground(StopForegroundFlags.Remove);
            base.OnDestroy();
        }
    }
}