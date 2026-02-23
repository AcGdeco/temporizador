using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Temporizador
{
    // Mensagens para comunicação entre TimerService e UI
    public class PararTimerPelaNotificacaoMessage { }
    public class ResetarTimerPelaNotificacaoMessage { }
    public class IniciarTimerPelaNotificacaoMessage { }
    public class PausarTimerPelaNotificacaoMessage { }
    
    // Mensagem para atualizar notificação
    public class AtualizarNotificacaoMessage 
    { 
        public string NovoTempo { get; set; }
        public bool EstaRodando { get; set; }
    }
}
