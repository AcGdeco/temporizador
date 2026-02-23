using global::Android.App;
using global::Android.Content;
using AndroidX.Core.App;
using static AndroidX.Core.App.NotificationCompat;
using Temporizador; // Para MainActivity

namespace Temporizador.Platforms.Android
{
    public static class TimerNotificationBuilder
    {
        private const string ChannelId = "timer_channel";

        public static NotificationCompat.Builder Build(Context context, string botao1Texto, string botao2Texto, string tempoAtual, long when = 0, bool chronometerCountDown = false)
        {
            var intent = new Intent(context, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            // Usando um ícone que com certeza existe no Android
            int smallIcon = GetSmallIcon(context);
            
            var builder = new NotificationCompat.Builder(context, ChannelId)
                .SetContentTitle("Temporizador")
                .SetContentText($"Tempo: {tempoAtual}")
                .SetSmallIcon(smallIcon)
                .SetContentIntent(pendingIntent)
                .SetPriority(NotificationCompat.PriorityLow)
                .SetSilent(true)
                .SetOngoing(true);

            if (when > 0 && chronometerCountDown)
            {
                builder.SetWhen(when);
                builder.SetUsesChronometer(true);
                if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.N)
                {
                    builder.SetChronometerCountDown(true);
                }
            }
            else
            {
                builder.SetShowWhen(false);
            }

            // Botão 1 (esquerda) - SEM ÍCONE
            // Envia como BROADCAST para o BotaoReceiver — garante funcionamento consistente
            var intent1 = new Intent(context, typeof(BotaoReceiver));
            intent1.SetAction("ACAO_" + TimerService.GetActionForButton(botao1Texto));
            intent1.PutExtra("tempo", tempoAtual);
            var pendingIntent1 = PendingIntent.GetBroadcast(context, GetRequestCode(botao1Texto), intent1, 
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            
            var action1 = new NotificationCompat.Action.Builder(
                0,  // 0 = sem ícone
                botao1Texto,
                pendingIntent1)
                .Build();

            // Botão 2 (direita) - SEM ÍCONE
            // Envia como BROADCAST para o BotaoReceiver — garante funcionamento consistente
            var intent2 = new Intent(context, typeof(BotaoReceiver));
            intent2.SetAction("ACAO_" + TimerService.GetActionForButton(botao2Texto));
            intent2.PutExtra("tempo", tempoAtual);
            var pendingIntent2 = PendingIntent.GetBroadcast(context, GetRequestCode(botao2Texto) + 100, intent2, 
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            
            var action2 = new NotificationCompat.Action.Builder(
                0,  // 0 = sem ícone
                botao2Texto,
                pendingIntent2)
                .Build();

            builder.AddAction(action1);
            builder.AddAction(action2);

            return builder;
        }

        private static int GetSmallIcon(Context context)
        {
            // Tenta usar o ícone do aplicativo (sempre existe)
            try
            {
                // Primeira tentativa: ícone do app
                int iconId = context.ApplicationInfo.Icon;
                if (iconId != 0)
                    return iconId;
            }
            catch { }

            // Fallback para ícone de sistema garantido
            return global::Android.Resource.Drawable.IcLockIdleAlarm;
        }

        private static int GetRequestCode(string texto)
        {
            return texto switch
            {
                "Pausar" => 1001,
                "Iniciar" => 1002,
                "Continuar" => 1003,
                "Parar" => 1004,
                "Reset" => 1005,
                _ => 2000
            };
        }
    }
}