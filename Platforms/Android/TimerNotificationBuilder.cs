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

            // PendingIntents
            var pararIntent = new Intent(context, typeof(BotaoReceiver)).SetAction("ACAO_PARAR");
            var iniciarIntent = new Intent(context, typeof(BotaoReceiver)).SetAction("ACAO_INICIAR");
            var pausarIntent = new Intent(context, typeof(BotaoReceiver)).SetAction("ACAO_PAUSAR");
            var resetarIntent = new Intent(context, typeof(BotaoReceiver)).SetAction("ACAO_RESETAR");

            var pararPI = PendingIntent.GetBroadcast(context, 0, pararIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            var iniciarPI = PendingIntent.GetBroadcast(context, 1, iniciarIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            var pausarPI = PendingIntent.GetBroadcast(context, 2, pausarIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            var resetarPI = PendingIntent.GetBroadcast(context, 3, resetarIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var abrirAppPI = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(MainActivity)), PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var mediaStyle = new MediaNotificationCompat.MediaStyle().SetShowActionsInCompactView(0, 1);

            // Selecionar ícones e PendingIntent baseado nos rótulos dos botões
            int btn1;
            int btn2;
            PendingIntent btn1PI;
            PendingIntent btn2PI;

            if(btn2Label == "Parar")
            {
                btn2 = Resource.Drawable.ic_stop;
                btn2PI = pararPI;
            }
            else if(btn2Label == "Reset")
            {
                btn2 = Resource.Drawable.ic_reset;
                btn2PI = resetarPI;
            }
            else
            {
                btn2 = Resource.Drawable.ic_reset_stop;
                btn2PI = pararPI;
            }

            if(btn1Label == "Pausar")
            {
                btn1 = Resource.Drawable.ic_pause;
                btn1PI = pausarPI; // Quando está "Pausar", clicar envia ACAO_PAUSAR
            }
            else if(btn1Label == "Continuar")
            {
                btn1 = Resource.Drawable.ic_play;
                btn1PI = iniciarPI; // Quando está "Continuar", clicar envia ACAO_INICIAR
            }
            else if(btn1Label == "Iniciar")
            {
                btn1 = Resource.Drawable.ic_play;
                btn1PI = iniciarPI; // Quando está "Iniciar", clicar envia ACAO_INICIAR
            }
            else
            {
                btn1 = Resource.Drawable.ic_pause_play;
                btn1PI = iniciarPI;
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
                .AddAction(btn2, btn2Label, btn2PI)
                .AddAction(btn1, btn1Label, btn1PI)
                .SetColor(GraphicsColor.Blue);
        }
    }
}