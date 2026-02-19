using Android.Content;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;

namespace Temporizador.Platforms.Android
{
    // Esta classe transporta os dados da MainPage para o TimerService
    public class AtualizarNotificacaoMessage 
    { 
        public string NovoTempo { get; set; } // Ex: "00:00"
        public bool EstaRodando { get; set; } // Define se a notificação é fixa (true) ou cancelável (false)
    }

    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class BotaoReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Sempre tente iniciar/awaken o TimerService com a mesma action — garante
            // que o serviço trate o comando mesmo quando a UI/Activity não está ativa.
            try
            {
                if (intent?.Action != null)
                {
                    var svcIntent = new Intent(context, typeof(TimerService));
                    svcIntent.SetAction(intent.Action);

n                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        context.StartForegroundService(svcIntent);
                    else
                        context.StartService(svcIntent);

n                    System.Diagnostics.Debug.WriteLine($"BotaoReceiver: started TimerService for action '{intent.Action}'");
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

    // Defina as mensagens como classes vazias para manter compatibilidade
    public class PararTimerPelaNotificacaoMessage { }
    public class IniciarTimerPelaNotificacaoMessage { }
    public class PausarTimerPelaNotificacaoMessage { }
    public class ResetarTimerPelaNotificacaoMessage { }
}