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

        public override void OnCreate()
        {
            base.OnCreate();

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);
            _alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);
            _handler = new Handler(Looper.MainLooper);

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
            if (intent?.Action != null)
            {
                switch (intent.Action)
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

            // Atualiza a notificação
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
            long remaining = _endTimeMillis - now;

            if (remaining <= 0) return;

            // Agenda para o próximo segundo cheio
            long nextUpdate = now + 1000 - (now % 1000);
            if (nextUpdate >= _endTimeMillis) return;

            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                {
                    _alarmManager.Set(AlarmType.ElapsedRealtime, nextUpdate, _updateIntent);
                }
                else
                {
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
            _handler.PostDelayed(() => {
                if (_estaRodando)
                {
                    HandleUpdate();
                    ScheduleUpdateWithHandler();
                }
            }, HandlerUpdateIntervalMs);
        }

        private void HandleUpdate()
        {
            if (!_estaRodando) return;

            long now = SystemClock.ElapsedRealtime();
            long remaining = _endTimeMillis - now;

            // Throttling manual
            if (now - _lastUpdateTime < MinUpdateIntervalMs && remaining > 1000)
                return;

            _lastUpdateTime = now;

            if (remaining <= 0)
            {
                PararTimer();
                return;
            }

            _tempoRestante = TimeSpan.FromMilliseconds(remaining);
            
            // Atualiza a notificação
            UpdateNotification();
            
            // Agenda próxima atualização
            ScheduleNextUpdate();
        }

        private void UpdateNotification()
        {
            var notification = BuildNotification();
            
            if (_isForeground)
            {
                _notificationManager.Notify(NotificationId, notification);
            }
        }

        private Notification BuildNotification()
        {
            string tempoFormatado = FormatTimeForNotification(_tempoRestante);
            string textoBotao1 = _estaRodando ? "Pausar" : "Iniciar";
            string textoBotao2 = _estaRodando ? "Parar" : "Reset";

            return TimerNotificationBuilder.Build(this, textoBotao1, textoBotao2, tempoFormatado)
                .SetContentTitle(_estaRodando ? "Temporizador em andamento" : "Temporizador parado")
                .SetContentText(tempoFormatado)
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
            
            UpdateNotification();
            
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
            
            WeakReferenceMessenger.Default.Unregister<PausarTimerPelaNotificacaoMessage>(this);
            WeakReferenceMessenger.Default.Unregister<IniciarTimerPelaNotificacaoMessage>(this);
            WeakReferenceMessenger.Default.Unregister<PararTimerPelaNotificacaoMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ResetarTimerPelaNotificacaoMessage>(this);
            
            StopForeground(StopForegroundFlags.Remove);
            base.OnDestroy();
        }
    }
}