using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Temporizador.Platforms.Android; // Onde está TimerNotificationBuilder
using CommunityToolkit.Mvvm.Messaging;
using CoreNotificationCompat = AndroidX.Core.App.NotificationCompat;
using AndroidX.Media.App; // Para MediaStyle, se ainda usar

namespace Temporizador
{
    [Service(Exported = true, ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
    public class TimerService : Service
    {
        private const int NotificationId = 1;
        private const string ChannelId = "timer_channel";

        public const string ActionParar = "ACAO_PARAR";
        public const string ActionIniciar = "ACAO_INICIAR";

        private CoreNotificationCompat.Builder _builder;
        private NotificationManager _notificationManager;
        private bool _isForeground = false;

        public override IBinder OnBind(Intent intent) => null;

        public override void OnCreate()
        {
            base.OnCreate();

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);

            // Cria (ou recria) o builder usando o helper externo
            _builder = TimerNotificationBuilder.Build(this, "Pausar", "Parar");

            // Registra o messenger (se ainda usar)
            WeakReferenceMessenger.Default.Register<AtualizarNotificacaoMessage>(
                this,
                (r, m) => AtualizarNotificacaoFisica(m.NovoTempo, m.EstaRodando));
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var tempo = intent?.GetStringExtra("tempo") ?? "00:00:00";
            AtualizarTexto(tempo);

            // Inicia como foreground imediatamente (importante!)
            var notification = _builder.Build();
            StartForeground(NotificationId, notification);
            _isForeground = true;

            return StartCommandResult.Sticky;
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
            WeakReferenceMessenger.Default.Unregister<AtualizarNotificacaoMessage>(this);
            StopForeground(StopForegroundFlags.Remove);
            base.OnDestroy();
        }
    }
}