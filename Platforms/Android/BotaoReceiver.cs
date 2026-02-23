using global::Android.Content;
using global::Android.Widget;
using CommunityToolkit.Mvvm.Messaging;
using Temporizador;

namespace Temporizador.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class BotaoReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            // Sempre tente iniciar/awaken o TimerService com a mesma action — garante
            // que o serviço trate o comando mesmo quando a UI/Activity não está ativa.
            try
            {
                if (intent?.Action != null && context != null)
                {
                    var svcIntent = new Intent(context, typeof(TimerService));
                    svcIntent.SetAction(intent.Action);
                    // Copia extras do broadcast (ex.: "tempo") para que o Service tenha o estado atual
                    if (intent?.Extras != null)
                    {
                        svcIntent.PutExtras(intent.Extras);
                    }

                    if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
                        context.StartForegroundService(svcIntent);
                    else
                        context.StartService(svcIntent);

                    System.Diagnostics.Debug.WriteLine($"BotaoReceiver: started TimerService for action '{intent.Action}' (extras copied: {intent?.Extras != null})");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BotaoReceiver: failed to start service: {ex.Message}");
            }

            // Também envie a mensagem via Messenger para atualizar UI se houver destinatário
            if (intent?.Action == "ACAO_PARAR")
            {
                WeakReferenceMessenger.Default.Send(new PararTimerPelaNotificacaoMessage());
            }
            else if (intent?.Action == "ACAO_INICIAR")
            {
                WeakReferenceMessenger.Default.Send(new IniciarTimerPelaNotificacaoMessage());
            }
            else if (intent?.Action == "ACAO_PAUSAR")
            {
                WeakReferenceMessenger.Default.Send(new PausarTimerPelaNotificacaoMessage());
            }
            else if (intent?.Action == "ACAO_RESETAR")
            {
                WeakReferenceMessenger.Default.Send(new ResetarTimerPelaNotificacaoMessage());
            }
        }
    }
}