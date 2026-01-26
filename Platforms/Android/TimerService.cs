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
using System.Timers;
using Stream = Android.Media.Stream;

namespace Temporizador
{
    [Service(Exported = true, ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync | Android.Content.PM.ForegroundService.TypeMediaPlayback)]
    public class TimerService : Service
    {
        private const int NotificationId = 1;
        private const string ChannelId = "timer_channel";

        public const string ActionParar = "ACAO_PARAR";
        public const string ActionIniciar = "ACAO_INICIAR";

        private CoreNotificationCompat.Builder _builder;
        private NotificationManager _notificationManager;
        private bool _isForeground = false;

        // ✅ Novos campos para timer integrado e alarme
        private System.Timers.Timer _timer;
        private TimeSpan _tempoRestante;
        private bool _estaRodando = false;
        private PowerManager.WakeLock _wakeLock;
        private AudioManager _audioManager;
        private Vibrator _vibrator;
        private MediaPlayer _mediaPlayer;

        public override IBinder OnBind(Intent intent) => null;

        public override void OnCreate()
        {
            base.OnCreate();

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);
            _audioManager = (AudioManager)GetSystemService(Context.AudioService);
            _vibrator = (Vibrator)GetSystemService(Context.VibratorService);

            // Cria (ou recria) o builder usando o helper externo
            _builder = TimerNotificationBuilder.Build(this, "Pausar", "Parar");

            // Registra o messenger (se ainda usar)
            WeakReferenceMessenger.Default.Register<AtualizarNotificacaoMessage>(
                this,
                (r, m) => AtualizarNotificacaoFisica(m.NovoTempo, m.EstaRodando));

            // ✅ Inicializa o timer
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var tempo = intent?.GetStringExtra("tempo") ?? "00:00:00";
            var rodando = intent?.GetBooleanExtra("estaRodando", false) ?? false;

            if (TimeSpan.TryParse(tempo, out var ts))
            {
                _tempoRestante = ts;
            }

            _estaRodando = rodando;

            // ✅ Inicia o timer se estiver rodando
            if (_estaRodando && !_timer.Enabled)
            {
                AcquireWakeLock();
                _timer.Start();
            }

            AtualizarTexto(tempo);

            // 🔑 Mantém o serviço ativo mesmo com tela bloqueada
            var notification = _builder.Build();
            StartForeground(NotificationId, notification);
            _isForeground = true;

            return StartCommandResult.Sticky;
        }

        private void AcquireWakeLock()
        {
            try
            {
                if (_wakeLock != null && _wakeLock.IsHeld)
                    return;

                PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
                _wakeLock = pm.NewWakeLock(
                    WakeLockFlags.Partial,
                    "Temporizador:Timer"
                );
                _wakeLock.Acquire();

                System.Diagnostics.Debug.WriteLine("WakeLock adquirido");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adquirir WakeLock: {ex.Message}");
            }
        }

        private void ReleaseWakeLock()
        {
            try
            {
                if (_wakeLock != null && _wakeLock.IsHeld)
                {
                    _wakeLock.Release();
                    System.Diagnostics.Debug.WriteLine("WakeLock liberado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao liberar WakeLock: {ex.Message}");
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_estaRodando && _tempoRestante.TotalSeconds > 0)
            {
                _tempoRestante = _tempoRestante.Subtract(TimeSpan.FromSeconds(1));

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AtualizarTexto(_tempoRestante.ToString(@"hh\:mm\:ss"));
                });
            }
            else if (_estaRodando && _tempoRestante.TotalSeconds <= 0)
            {
                _estaRodando = false;
                _timer?.Stop();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PlayAlarmWithScreenLocked();
                    VibratePattern();
                });
            }
        }

        // ✅ Toca o alarme mesmo com tela bloqueada
        private void PlayAlarmWithScreenLocked()
        {
            try
            {
                // Define volume máximo
                _audioManager?.SetStreamVolume(
                    Stream.Alarm,
                    _audioManager.GetStreamMaxVolume(Stream.Alarm),
                    VolumeNotificationFlags.ShowUi
               );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao tocar alarme: {ex.Message}");
            }
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
            _builder.SetContentText($"Tempo restante: {tempo}");
            var notification = _builder.Build();

            if (!_isForeground)
            {
                StartForeground(NotificationId, notification);
                _isForeground = true;
            }
            else
            {
                _notificationManager.Notify(NotificationId, notification);
            }
        }

        private void AtualizarNotificacaoFisica(string tempo, bool rodando)
        {
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

        public override void OnDestroy()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _mediaPlayer?.Stop();
            _mediaPlayer?.Release();
            ReleaseWakeLock();

            WeakReferenceMessenger.Default.Unregister<AtualizarNotificacaoMessage>(this);
            StopForeground(StopForegroundFlags.Remove);
            base.OnDestroy();
        }
    }
}