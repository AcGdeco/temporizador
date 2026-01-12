using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Media.App;
using Temporizador.Platforms.Android;
using CommunityToolkit.Mvvm.Messaging; // Adicionado
using CoreNotificationCompat = AndroidX.Core.App.NotificationCompat;
using Android.Graphics;
using OsBuild = Android.OS.Build; 
using OsVersionCodes = Android.OS.BuildVersionCodes;
using MediaNotificationCompat = AndroidX.Media.App.NotificationCompat;
using Android.Widget; 
using Android.Content;

// Adicione a diretiva de namespace explicitamente para evitar ambiguidade
using GraphicsColor = Android.Graphics.Color;

namespace Temporizador.Platforms.Android
{
    public class TimerNotificationBuilder
    {
        public static AndroidX.Core.App.NotificationCompat.Builder Build(Context context, string btn1Label, string btn2Label)
        {
            // Canal
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            if (OsBuild.VERSION.SdkInt >= OsVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    "timer_channel",
                    "Temporizador",
                    NotificationImportance.Low)
                {
                    Description = "Canal do temporizador",
                    LockscreenVisibility = NotificationVisibility.Public
                };
                notificationManager.CreateNotificationChannel(channel);
            }

            // PendingIntents (copie exatamente como no original)
            var pararIntent = new Intent(context, typeof(BotaoReceiver)).SetAction("ACAO_PARAR");
            var iniciarIntent = new Intent(context, typeof(BotaoReceiver)).SetAction("ACAO_INICIAR");

            var pararPI = PendingIntent.GetBroadcast(context, 0, pararIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            var iniciarPI = PendingIntent.GetBroadcast(context, 1, iniciarIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var abrirAppPI = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(MainActivity)), PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var mediaStyle = new MediaNotificationCompat.MediaStyle().SetShowActionsInCompactView(0, 1);

            // btn1 e btn2 devem ser int, n�o string
            int btn1;
            int btn2;

            if(btn2Label == "Parar")
            {
                btn2 = Resource.Drawable.ic_reset_stop;
            }
            else
            {
                btn2 = Resource.Drawable.ic_reset_stop;
            }

            if(btn1Label == "Pausar")
            {
                btn1 = Resource.Drawable.ic_pause_play;
            }
            else
            {
                btn1 = Resource.Drawable.ic_pause_play;
            }

            return new AndroidX.Core.App.NotificationCompat.Builder(context, "timer_channel")
                .SetSmallIcon(Resource.Drawable.ic_timer)
                .SetContentTitle("Temporizador em andamento")
                .SetContentText("Tempo restante")
                .SetContentIntent(abrirAppPI)
                .SetStyle(mediaStyle)
                .SetPriority(AndroidX.Core.App.NotificationCompat.PriorityLow)
                .SetCategory(Notification.CategoryService)
                .SetVisibility(AndroidX.Core.App.NotificationCompat.VisibilityPublic)
                .SetOngoing(true)
                .SetOnlyAlertOnce(true)
                .SetForegroundServiceBehavior(AndroidX.Core.App.NotificationCompat.ForegroundServiceImmediate)
                .AddAction(btn2, btn2Label, pararPI)
                .AddAction(btn1, btn1Label, iniciarPI)
                .SetColor(GraphicsColor.Blue); // Corrigido para usar Android.Graphics.Color.Blue explicitamente
        }
    }
}