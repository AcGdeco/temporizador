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
            if (intent.Action == "ACAO_PARAR")
            {
                WeakReferenceMessenger.Default.Send(new PararTimerPelaNotificacaoMessage());
            }
            else if (intent.Action == "ACAO_INICIAR")
            {
                WeakReferenceMessenger.Default.Send(new IniciarTimerPelaNotificacaoMessage());
            }            
        }
    }

    // Defina as mensagens como classes vazias para manter compatibilidade
    public class PararTimerPelaNotificacaoMessage { }
    public class IniciarTimerPelaNotificacaoMessage { }
}