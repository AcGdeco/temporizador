using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using AndroidX.Core.App;
using Temporizador.Platforms.Android;
using CommunityToolkit.Mvvm.Messaging;
using CoreNotificationCompat = AndroidX.Core.App.NotificationCompat;
using AndroidX.Media.App;
using Microsoft.Maui.Dispatching;
using System;

namespace Temporizador
{
    [Service(Exported = true, ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
    public class TimerService : Service
    {
        private const int NotificationId = 1;
        private const string ChannelId = "timer_channel";

        public const string ActionParar = "ACAO_PARAR";
        public const string ActionIniciar = "ACAO_INICIAR";

        private const string ActionUpdate = "UPDATE_NOTIFICATION";
        private const string ActionExpire = "EXPIRE_TIMER";

        private const int RequestCodeUpdate = 1;
        private const int RequestCodeExpire = 2;

        private CoreNotificationCompat.Builder _builder;
        private NotificationManager _notificationManager;
        private bool _isForeground = false;

        // Notification update debounce — evita flood de notificações e rate limiting do sistema
        private string _lastNotifiedText = string.Empty;
        private long _lastNotifyElapsed = 0; // SystemClock.ElapsedRealtime()
        private const long MinNotifyIntervalMs = 1000; // mínimo entre Notify()s quando texto não muda

        // ✅ Novos campos para gerenciamento otimizado
        private long _endElapsedMillis;
        private TimeSpan _tempoRestante;
        private TimeSpan _tempoInicial;  // ✅ Armazena o tempo definido antes de iniciar
        private bool _estaRodando = false;
        private AudioManager _audioManager;
        private Vibrator _vibrator;
        private AlarmManager _alarmManager;

        public override IBinder OnBind(Intent intent) => null;

        public override void OnCreate()
        {
            base.OnCreate();

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);
            _audioManager = (AudioManager)GetSystemService(Context.AudioService);
            _vibrator = (Vibrator)GetSystemService(Context.VibratorService);
            _alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);

            // Cria (ou recria) o builder usando o helper externo
            _builder = TimerNotificationBuilder.Build(this, "Pausar", "Parar");

            // Registra o messenger (se ainda usar)
            WeakReferenceMessenger.Default.Register<AtualizarNotificacaoMessage>(
                this,
                (r, m) => AtualizarNotificacaoFisica(m.NovoTempo, m.EstaRodando));

            WeakReferenceMessenger.Default.Register<PausarTimerPelaNotificacaoMessage>(
                this,
                (r, m) => PausarTimer());

            WeakReferenceMessenger.Default.Register<IniciarTimerPelaNotificacaoMessage>(
                this,
                (r, m) => RetomarTimer());

            WeakReferenceMessenger.Default.Register<PararTimerPelaNotificacaoMessage>(
                this,
                (r, m) => PararTimer());

            WeakReferenceMessenger.Default.Register<ResetarTimerPelaNotificacaoMessage>(
                this,
                (r, m) => ResetarTimer());
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (intent != null && intent.Action != null)
            {
                switch (intent.Action)
                {
                    case ActionUpdate:
                        HandleUpdate();
                        return StartCommandResult.Sticky;
                    case ActionExpire:
                        HandleExpire();
                        return StartCommandResult.Sticky;
                    case ActionParar:
                        // Ação vinda da notificação — pare imediatamente (garanta idempotência)
                        PararTimer();
                        return StartCommandResult.Sticky;
                    case ActionIniciar:
                        // Ação vinda da notificação — retome se possível
                        RetomarTimer();
                        return StartCommandResult.Sticky;
                }
            }

n            var tempo = intent?.GetStringExtra("tempo") ?? "00:00:00";
            var tempoInicialStr = intent?.GetStringExtra("tempoInicial") ?? tempo;  // ✅ Recebe tempo inicial
            var rodando = intent?.GetBooleanExtra("estaRodando", false) ?? false;

            if (TimeSpan.TryParse(tempo, out var ts))
            {
                _tempoRestante = ts;
            }
            
            // ✅ Salva o tempo inicial se foi passado, senão usa o tempo atual
            if (TimeSpan.TryParse(tempoInicialStr, out var tsInicial))
            {
                _tempoInicial = tsInicial;
            }
            else
            {
                _tempoInicial = _tempoRestante;
            }

            _estaRodando = rodando;

            // ✅ Reconstrói a notificação com os botões corretos baseado no novo estado
            if (rodando)
            {
                // Timer está rodando: mostrar "Pausar" e "Parar"
                _builder = TimerNotificationBuilder.Build(this, "Pausar", "Parar");
            }
            else
            {
                // Timer está parado: mostrar "Iniciar" e "Reset"
                _builder = TimerNotificationBuilder.Build(this, "Iniciar", "Reset");
            }
            
            _builder.SetContentText(FormatarTempoNotificacao(_tempoRestante));

            // ✅ Inicia o timer se estiver rodando
            if (_estaRodando)
            {
                StartTimerLogic();
            }

            AtualizarTexto(FormatarTempoNotificacao(_tempoRestante));

            // 🔑 Mantém o serviço ativo mesmo com tela bloqueada
            var notification = _builder.Build();
            StartForeground(NotificationId, notification);
            _isForeground = true;

            return StartCommandResult.Sticky;
        }

        private void StartTimerLogic()
        {
            var currentElapsed = SystemClock.ElapsedRealtime();
            _endElapsedMillis = currentElapsed + (long)_tempoRestante.TotalMilliseconds;
            _estaRodando = true;

            ScheduleExpireAlarm();
            HandleUpdate();  // Atualização inicial e agendamento do próximo
        }

        private void ScheduleExpireAlarm()
        {
            var pendingIntent = GetPendingIntent(ActionExpire, RequestCodeExpire);

            // Se for permitido, use alarm exato; caso contrário, caia para um alarm não-exato
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                {
                    // Sem permissão para alarms exatos — agendar não-exato para evitar SecurityException
                    _alarmManager.Set(AlarmType.ElapsedRealtime, _endElapsedMillis, pendingIntent);
                    System.Diagnostics.Debug.WriteLine("ScheduleExpireAlarm: falling back to inexact alarm (no exact-alarm permission)");
                    return;
                }

                // Caso permitido ou versão anterior ao Android 12, agende exato
                _alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, _endElapsedMillis, pendingIntent);
            }
            catch (Java.Lang.SecurityException ex)
            {
                // Proteção extra: se houver SecurityException, usar fallback inexact
                System.Diagnostics.Debug.WriteLine($"ScheduleExpireAlarm: SecurityException, using fallback alarm: {ex.Message}");
                _alarmManager.Set(AlarmType.ElapsedRealtime, _endElapsedMillis, pendingIntent);
            }
        }

        private void ScheduleNextUpdateAlarm()
        {
            var currentElapsed = SystemClock.ElapsedRealtime();
            var remainingMillis = _endElapsedMillis - currentElapsed;
            if (remainingMillis <= 0) return;

            const long mod = 60000;  // 1 minuto em ms
            var currentMod = currentElapsed % mod;
            var endMod = _endElapsedMillis % mod;
            var millisToNext = (endMod - currentMod + mod) % mod;
            if (millisToNext == 0) millisToNext = mod;

            var nextTrigger = currentElapsed + millisToNext;
            if (nextTrigger >= _endElapsedMillis) return;  // Não agendar se depois ou no fim

            var pendingIntent = GetPendingIntent(ActionUpdate, RequestCodeUpdate);

            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !_alarmManager.CanScheduleExactAlarms())
                {
                    _alarmManager.Set(AlarmType.ElapsedRealtime, nextTrigger, pendingIntent);
                    System.Diagnostics.Debug.WriteLine("ScheduleNextUpdateAlarm: falling back to inexact alarm (no exact-alarm permission)");
                }
                else
                {
                    _alarmManager.SetExact(AlarmType.ElapsedRealtimeWakeup, nextTrigger, pendingIntent);
                }
            }
            catch (Java.Lang.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine($"ScheduleNextUpdateAlarm: SecurityException, using fallback alarm: {ex.Message}");
                _alarmManager.Set(AlarmType.ElapsedRealtime, nextTrigger, pendingIntent);
            }
        }

        private void CancelUpdateAlarm()
        {
            var pendingIntent = GetPendingIntent(ActionUpdate, RequestCodeUpdate);
            _alarmManager.Cancel(pendingIntent);
        }

        private void CancelExpireAlarm()
        {
            var pendingIntent = GetPendingIntent(ActionExpire, RequestCodeExpire);
            _alarmManager.Cancel(pendingIntent);
        }

        private PendingIntent GetPendingIntent(string action, int requestCode)
        {
            var intent = new Intent(this, typeof(TimerService));
            intent.SetAction(action);
            return PendingIntent.GetService(this, requestCode, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
        }

        private TimeSpan GetRemainingTime()
        {
            if (!_estaRodando) return _tempoRestante;

            var remainingMillis = _endElapsedMillis - SystemClock.ElapsedRealtime();
            return remainingMillis > 0 ? TimeSpan.FromMilliseconds(remainingMillis) : TimeSpan.Zero;
        }

        private void HandleUpdate()
        {
            var remaining = GetRemainingTime();
            if (remaining <= TimeSpan.Zero)
            {
                HandleExpire();
                return;
            }

            AtualizarTexto(FormatarTempoNotificacao(remaining));
            ScheduleNextUpdateAlarm();
        }

        private void HandleExpire()
        {
            _estaRodando = false;
            _endElapsedMillis = 0;
            CancelUpdateAlarm();

            VibratePattern();

            // Atualiza notificação para estado expirado/parado
            _builder = TimerNotificationBuilder.Build(this, "Iniciar", "Reset");
            AtualizarTexto("Tempo esgotado");
        }

        // ✅ Formata o tempo como "menos que Xh : YY m" (arredonda minutos para CIMA se há segundos)
        private string FormatarTempoNotificacao(TimeSpan tempo)
        {
            int horas = (int)tempo.TotalHours;
            int minutos = tempo.Minutes;
            int segundos = tempo.Seconds;
            
            // ✅ Se há segundos, arredonda para cima (próximo minuto)
            if (segundos > 0)
            {
                minutos++;
                // Se ultrapassar 60 minutos, ajusta horas
                if (minutos >= 60)
                {
                    horas++;
                    minutos = 0;
                }
            }
            
            return $"menos que {horas}h : {minutos:D2}m";
        }

        private void VibratePattern()
        {
            try
            {
                if (_vibrator == null || !_vibrator.HasVibrator)
                    return;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    long[] pattern = { 0, 500, 300, 500, 300, 500 };
                    var effect = VibrationEffect.CreateWaveform(pattern, 0);
                    _vibrator.Vibrate(effect);
                }
                else
                {
#pragma warning disable CS0618
                    long[] pattern = { 0, 500, 300, 500, 300, 500 };
                    _vibrator.Vibrate(pattern, 0);
#pragma warning restore CS0618
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao vibrar: {ex.Message}");
            }
        }

        private void AtualizarTexto(string tempo)
        {
            // Debounce / no-op se o texto não mudou e aconteceu recentemente — evita rate limiting do sistema
            var now = SystemClock.ElapsedRealtime();
            if (_lastNotifiedText == tempo && (now - _lastNotifyElapsed) < MinNotifyIntervalMs)
            {
                System.Diagnostics.Debug.WriteLine($"AtualizarTexto: skip notify (rate-limited) — '{tempo}'");
                return;
            }

            _lastNotifiedText = tempo;
            _lastNotifyElapsed = now;

            _builder.SetContentText(tempo);
            var notification = _builder.Build();

            if (!_isForeground)
            {
                StartForeground(NotificationId, notification);
                _isForeground = true;
            }
            else
            {
                // Só notificar quando houver mudança relevante (ou intervalo mínimo expirou)
                System.Diagnostics.Debug.WriteLine($"AtualizarTexto: notifying — '{tempo}'");
                _notificationManager.Notify(NotificationId, notification);
            }
        }

        private void AtualizarNotificacaoFisica(string tempo, bool rodando)
        {
            // Debounce: combine texto+estado para evitar re-posts idênticos em curto intervalo
            var combined = $"{tempo}|{rodando}";
            var now = SystemClock.ElapsedRealtime();
            if (_lastNotifiedText == combined && (now - _lastNotifyElapsed) < MinNotifyIntervalMs)
            {
                System.Diagnostics.Debug.WriteLine($"AtualizarNotificacaoFisica: skip notify (rate-limited) — {combined}");
                return;
            }

            _lastNotifiedText = combined;
            _lastNotifyElapsed = now;

            // ✅ Reconstrói a notificação com os botões corretos baseado no estado
            if (rodando)
            {
                // Timer está rodando: mostrar "Pausar" e "Parar"
                _builder = TimerNotificationBuilder.Build(this, "Pausar", "Parar");
            }
            else
            {
                // Timer está parado: mostrar "Iniciar" e "Reset"
                _builder = TimerNotificationBuilder.Build(this, "Iniciar", "Reset");
            }
            
            _builder
                .SetContentTitle(rodando ? "Temporizador em andamento" : "Temporizador parado")
                .SetContentText($"Tempo: {tempo}")
                .SetOngoing(rodando);

            var notification = _builder.Build();
            _notificationManager.Notify(NotificationId, notification);

            if (!rodando && _isForeground)
            {
                StopForeground(StopForegroundFlags.Detach);
                _isForeground = false;
            }
        }

        private void PausarTimer()
        {
            if (!_estaRodando) return;

            var remaining = GetRemainingTime();
            _tempoRestante = remaining;
            _endElapsedMillis = 0;
            _estaRodando = false;
            CancelExpireAlarm();
            CancelUpdateAlarm();
            System.Diagnostics.Debug.WriteLine("Timer pausado pelo comando da notificação");
            
            // Reconstrói a notificação com o botão "Continuar"
            _builder = TimerNotificationBuilder.Build(this, "Continuar", "Parar");
            _builder.SetContentText(FormatarTempoNotificacao(_tempoRestante));
            var notification = _builder.Build();
            _notificationManager.Notify(NotificationId, notification);
        }

        private void RetomarTimer()
        {
            if (_tempoRestante.TotalSeconds > 0 && !_estaRodando)
            {
                StartTimerLogic();
                System.Diagnostics.Debug.WriteLine("Timer retomado pelo comando da notificação");
                
                // Reconstrói a notificação com o botão "Pausar"
                _builder = TimerNotificationBuilder.Build(this, "Pausar", "Parar");
                _builder.SetContentText(FormatarTempoNotificacao(GetRemainingTime()));
                var notification = _builder.Build();
                _notificationManager.Notify(NotificationId, notification);
            }
        }

        private void PararTimer()
        {
            _estaRodando = false;
            CancelExpireAlarm();
            CancelUpdateAlarm();
            _tempoRestante = _tempoInicial;  // ✅ Volta ao tempo definido antes de iniciar
            _endElapsedMillis = 0;
            
            // ✅ Cancela a vibração
            _vibrator?.Cancel();
            
            System.Diagnostics.Debug.WriteLine($"Timer parado pelo comando da notificação. Tempo restaurado para: {_tempoRestante.ToString(@"hh\:mm\:ss")}");
            
            // Reconstrói a notificação com os botões "Iniciar" e "Reset"
            _builder = TimerNotificationBuilder.Build(this, "Iniciar", "Reset");
            _builder.SetContentText(FormatarTempoNotificacao(_tempoRestante));
            var notification = _builder.Build();
            _notificationManager.Notify(NotificationId, notification);
            
            // ✅ Se não estamos mais em foreground (após parar), remova a flag
            if (_isForeground)
            {
                StopForeground(StopForegroundFlags.Detach);
                _isForeground = false;
            }
        }

        private void ResetarTimer()
        {
            _estaRodando = false;
            CancelExpireAlarm();
            CancelUpdateAlarm();
            _tempoRestante = TimeSpan.Zero;
            _endElapsedMillis = 0;
            System.Diagnostics.Debug.WriteLine("Timer resetado e notificação removida");
            
            // Remove a notificação completamente
            if (_isForeground)
            {
                StopForeground(StopForegroundFlags.Remove);
                _isForeground = false;
            }
            _notificationManager.Cancel(NotificationId);
        }

        public override void OnDestroy()
        {
            CancelExpireAlarm();
            CancelUpdateAlarm();

            WeakReferenceMessenger.Default.Unregister<AtualizarNotificacaoMessage>(this);
            StopForeground(StopForegroundFlags.Remove);
            base.OnDestroy();
        }
    }
}