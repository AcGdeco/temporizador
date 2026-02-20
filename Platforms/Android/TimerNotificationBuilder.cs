using Android.App;
using Android.Content;
using AndroidX.Core.App;
using static AndroidX.Core.App.NotificationCompat;

namespace Temporizador.Platforms.Android
{
    public static class TimerNotificationBuilder
    {
        private const string ChannelId = "timer_channel";

        public static NotificationCompat.Builder Build(Context context, string botao1Texto, string botao2Texto, string tempoAtual)
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

            // Botão 1 (esquerda) - SEM ÍCONE
            var intent1 = new Intent(context, typeof(TimerService));
            intent1.SetAction(TimerService.GetActionForButton(botao1Texto));
            intent1.PutExtra("tempo", tempoAtual);
            var pendingIntent1 = PendingIntent.GetService(context, GetRequestCode(botao1Texto), intent1, 
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            
            var action1 = new NotificationCompat.Action.Builder(
                0,  // 0 = sem ícone
                botao1Texto,
                pendingIntent1)
                .Build();

            // Botão 2 (direita) - SEM ÍCONE
            var intent2 = new Intent(context, typeof(TimerService));
            intent2.SetAction(TimerService.GetActionForButton(botao2Texto));
            intent2.PutExtra("tempo", tempoAtual);
            var pendingIntent2 = PendingIntent.GetService(context, GetRequestCode(botao2Texto) + 100, intent2, 
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

            // Segunda tentativa: usar um ícone do sistema Android
            try
            {
                // Usando reflexão para acessar o Resource do Android
                var drawableClass = Type.GetType("Android.Resource+Drawable, Mono.Android");
                if (drawableClass != null)
                {
                    var field = drawableClass.GetField("ic_dialog_info");
                    if (field != null)
                    {
                        var value = field.GetValue(null);
                        if (value is int intValue)
                            return intValue;
                    }
                }
            }
            catch { }

            // Último recurso: retorna 0 e deixa o Android escolher
            return 0;
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