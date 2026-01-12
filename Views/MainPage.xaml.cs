using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views; // necessário para ShowPopup/ShowPopupAsync
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using System;
using System.IO;
using System.Timers;
using Temporizador.Popups;   // onde está sua NovaReceitaPopup
using Temporizador.Models; 
using SQLite;
using Microsoft.Maui.Controls.Shapes;
using CommunityToolkit.Maui.Behaviors;
using Plugin.Maui.Audio; 
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Temporizador.Platforms.Android;
using Android.Widget;
using Android.Content;
using Temporizador.Platforms.Android;
#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
#endif

namespace Temporizador.Views
{
    public partial class MainPage : ContentPage
    {
        private SQLiteConnection _db;   // ✅ conexão com o banco

        private TimeSpan tempoInicial;
        private TimeSpan tempoRestante;
        private System.Timers.Timer timer;

        private enum EstadoTemporizador { Parado, Rodando, Pausado, Resetado }
        private EstadoTemporizador estadoTemporizador = EstadoTemporizador.Resetado;
        private TimeSpan tempoDefinido;

        private IAudioPlayer _alarmePlayer;
        private bool _audioInitialized = false;

        // BroadcastReceiver para ações da notificação
        private TimerBroadcastReceiver _receiver;
        private NotificationCompat.Builder _builder;

        public MainPage()
        {
            InitializeComponent();

            var btn1Label = "";
            var btn2Label = "";

            // Registro para PARAR
            WeakReferenceMessenger.Default.Register<PararTimerPelaNotificacaoMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (BotaoResetLabel.Text == "Parar")
                    {
                        OnResetClicked(null, null);
                        AtualizarNotificacaoAndroid(tempoDefinido.ToString("hh\\:mm\\:ss"));
                    } else {
                        OnResetClicked(null, null);
                    }

                    if (BotaoResetLabel.Text == "Parar")
                    {
                        btn2Label = "Parar";
                    } else {
                        btn2Label = "Resetar";
                    }

                    btn1Label = BotaoIniciarLabel.Text;
                    // Cria (ou recria) o builder usando o helper externo
                    var context = Android.App.Application.Context;
                    _builder = TimerNotificationBuilder.Build(context, btn1Label, btn2Label);

                });
            });

            // Registro para INICIAR
            WeakReferenceMessenger.Default.Register<IniciarTimerPelaNotificacaoMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    OnStartClicked(null, null);

                    if (BotaoIniciarLabel.Text == "Pausar")
                    {
                        btn1Label = "Pausar";
                    } else {
                        btn1Label = "Continuar";
                    }

                    btn2Label = BotaoResetLabel.Text;
                    // Cria (ou recria) o builder usando o helper externo
                    var context = Android.App.Application.Context;
                    _builder = TimerNotificationBuilder.Build(context, btn1Label, btn2Label);

                });
            });

            // ✅ inicializa o banco
            var dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "receitas.db");
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Receita>();

            tempoInicial = TimeSpan.Zero;
            tempoRestante = tempoInicial;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
        }

        private void AtualizarNotificacaoAndroid(string tempoFormatado)
        {
        #if ANDROID
            try
            {
                var intent = new Intent(Android.App.Application.Context, typeof(TimerService));
                intent.PutExtra("tempo", tempoFormatado);
                intent.PutExtra("estaRodando", estadoTemporizador == EstadoTemporizador.Rodando);
                intent.PutExtra("btn1Label", BotaoIniciarLabel.Text);   // "Pausar" ou "Continuar"
                intent.PutExtra("btn2Label", BotaoResetLabel.Text);     // "Parar" ou "Reset"
                Android.App.Application.Context.StartForegroundService(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar notificação: {ex.Message}");
            }
        #endif
        }

        private void StopNotificationAndroid()
        {
        #if ANDROID
            var intent = new Intent(Android.App.Application.Context, typeof(TimerService));
            Android.App.Application.Context.StopService(intent);
        #endif
        }

        private async void OnPageLoaded(object sender, EventArgs e)
        {
            bool close = false;
            if (!Preferences.Get("test_version", true) && close == true)
            {
                await DisplayAlert("Teste", "Versão de Teste, o programa só pode ser aberto apenas 1 vez, além disso o programa sempre vai ser fechado ao abrir, desinstale e o instale novamente para usá-lo!", "OK");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            Preferences.Set("test_version", false);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Só inicializa o áudio uma vez
                if (!_audioInitialized)
                {
                    await InitializeAudioPlayerAsync();
                    _audioInitialized = true;
                }

                // Carrega as receitas do banco de forma segura
                CarregarReceitas();

                AtualizarUI();
            }
            catch (Exception ex)
            {
    #if DEBUG
                await DisplayAlert("Erro no OnAppearing", ex.Message, "OK");
    #endif
            }
        }

        private async Task InitializeAudioPlayerAsync()
        {
            try
            {
                var audioManager = AudioManager.Current;

                // Agora sim, await de verdade (não bloqueia)
                await using var stream = await FileSystem.OpenAppPackageFileAsync("alarme.mp3");

                _alarmePlayer = audioManager.CreatePlayer(stream);

                // Configura loop
                _alarmePlayer.Loop = true;

                // Opcional: pré-carrega se quiser, mas geralmente não precisa
                // _alarmePlayer.Play(); _alarmePlayer.Pause();
            }
            catch (Exception ex)
            {
                // Trate o erro (arquivo não encontrado, formato inválido, etc.)
                System.Diagnostics.Debug.WriteLine($"Falha ao carregar alarme: {ex.Message}");
    #if DEBUG
                await DisplayAlert("Erro no áudio", "Não foi possível carregar o alarme.mp3", "OK");
    #endif
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void CarregarReceitas()
        {
            ReceitasContainer.Children.Clear();
            ReceitasContainer.RowDefinitions.Clear();

            var receitas = _db.Table<Receita>().ToList();

            int row = 0, col = 0;

            foreach (var receita in receitas)
            {
                if (ReceitasContainer.RowDefinitions.Count <= row)
                    ReceitasContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var corBase = Color.FromArgb(receita.Cor);

                string corComAlpha = "#1A" + receita.Cor.Substring(1); 

                var frame = new Frame
                {
                    BackgroundColor = Color.FromArgb(corComAlpha),  
                    BorderColor = corBase,
                    CornerRadius = 8,
                    HasShadow = false,
                    Padding = new Thickness(10, 14),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BindingContext = receita // importante para saber qual receita foi clicada
                };

                var grid = new Grid
                {
                    RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    },
                    RowSpacing = 2
                };

                var nomeLabel = new Label
                {
                    Text = receita.Nome,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center
                };
                var tempoLabel = new Label
                {
                    Text = TimeSpan.FromSeconds(receita.Tempo).ToString(@"hh\:mm\:ss"),
                    FontSize = 14,
                    TextColor = Colors.Black,
                    HorizontalOptions = LayoutOptions.Center
                };
                grid.Add(nomeLabel, 0, 0);
                grid.Add(tempoLabel, 0, 1);

                frame.Content = grid;

                // Tap normal → seleciona receita
                var tap = new TapGestureRecognizer();
                tap.Tapped += OnReceitaSelecionada;
                frame.GestureRecognizers.Add(tap);
                frame.BindingContext = receita;

                // Remove qualquer gesto antigo
                frame.GestureRecognizers.Clear();
                frame.Behaviors.Clear();

                // NOVO: TouchBehavior com toque normal e long press separados
                var touchBehavior = new TouchBehavior
                {
                    Command = new Command(() =>
                    {
                        // Toque normal → seleciona a receita e define o temporizador
                        OnReceitaSelecionada(frame, EventArgs.Empty);
                    }),
                    LongPressCommand = new Command(async () =>
                    {
                        // Pressão longa → abre edição (NÃO altera o tempo!)
                        var popup = new EditarReceitaPopup(receita, _db);
                        var resultado = await this.ShowPopupAsync(popup);
                        // Recarrega os cards após salvar ou deletar
                        CarregarReceitas();
                    }),
                    LongPressDuration = 600, // 0.6s pra ser bem confortável
                };
                frame.Behaviors.Add(touchBehavior);

                ReceitasContainer.Add(frame, col, row);

                col++;
                if (col > 1) { col = 0; row++; }
            }

            // card "Nova Receita"
            if (ReceitasContainer.RowDefinitions.Count <= row)
                ReceitasContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var novaReceitaBorder = new Border
            {
                Stroke = Color.FromArgb("#666666"),
                StrokeThickness = 1,
                BackgroundColor = Colors.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 14),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
                // Define padrão pontilhado: traço curto + espaço
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };

            var novaGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                RowSpacing = 2
            };

            novaGrid.Add(new Label
            {
                Text = "+",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 0);

            novaGrid.Add(new Label
            {
                Text = "Nova Receita",
                FontSize = 14,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 1);

            novaReceitaBorder.Content = novaGrid;

            var tapNova = new TapGestureRecognizer();
            tapNova.Tapped += OnNovaReceitaClicked;
            novaReceitaBorder.GestureRecognizers.Add(tapNova);

            ReceitasContainer.Add(novaReceitaBorder, col, row);
        }

        private void OnReceitaLongPress(Receita receita)
        {
            // aqui você abre popup de edição/deletar
            var popup = new EditarReceitaPopup(receita, _db);
            this.ShowPopup(popup);
        }

        private void AnimarTempoRestanteLabel()
        {
            // Inicia a animação sem esperar (fire-and-forget)
            TempoRestanteLabel.ScaleTo(0.95, 1, Easing.CubicOut)
                .ContinueWith(t =>
                {
                    TempoRestanteLabel.ScaleTo(1.0, 90, Easing.CubicIn);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AtualizarUI()
        {
            // Atualiza o texto
            TempoRestanteLabel.Text = tempoRestante.ToString(@"hh\:mm\:ss");

            // Anima o texto (pode manter)
            AnimarTempoRestanteLabel();

            // Largura total da barra
            double larguraTotal = BarraContainer.Width;
            if (larguraTotal <= 0)
                larguraTotal = this.Width - 40;

            // Evita divisão por zero
            double progresso = tempoInicial.TotalSeconds > 0
                ? tempoRestante.TotalSeconds / tempoInicial.TotalSeconds
                : 0;

            double novaLargura = larguraTotal * progresso;

            // ✅ 1. RODANDO → anima a barra diminuindo
            if (estadoTemporizador == EstadoTemporizador.Rodando)
            {
                BarraLaranja.Animate(
                    "Progresso",
                    x => BarraLaranja.WidthRequest = x,
                    BarraLaranja.WidthRequest,
                    novaLargura,
                    length: 1000,
                    easing: Easing.Linear
                );
                return;
            }

            // ✅ 2. PAUSADO → não anima, só mantém a largura atual
            if (estadoTemporizador == EstadoTemporizador.Pausado)
            {
                // Não anima nada, só mantém o valor atual
                BarraLaranja.WidthRequest = BarraLaranja.WidthRequest;
                return;
            }

            // ✅ 3. PARADO → volta ao tempo inicial e barra cheia
            if (estadoTemporizador == EstadoTemporizador.Parado)
            {
                BarraLaranja.AbortAnimation("Progresso");
                BarraLaranja.WidthRequest = larguraTotal;
                return;
            }

            // ✅ 4. RESETADO → tempo zerado e barra zerada
            if (estadoTemporizador == EstadoTemporizador.Resetado)
            {
                BarraLaranja.WidthRequest = 0;
                return;
            }
        }

        private void OnAddTimeClicked(object sender, EventArgs e)
        {
            estadoTemporizador = EstadoTemporizador.Parado;
           
            if (sender is Border border &&
                border.GestureRecognizers[0] is TapGestureRecognizer tap &&
                tap.CommandParameter is string s &&
                int.TryParse(s, out int addSeg))
            {
                tempoRestante = tempoRestante.Add(TimeSpan.FromSeconds(addSeg));
                tempoInicial = tempoInicial.Add(TimeSpan.FromSeconds(addSeg));
                tempoDefinido = tempoInicial;
                AtualizarUI();
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            if (BotaoResetLabel.Text == "Reset")
            {
                tempoInicial = TimeSpan.Zero;
                tempoRestante = TimeSpan.Zero;
                estadoTemporizador = EstadoTemporizador.Resetado;
                StopNotificationAndroid();
            }
            else if (BotaoResetLabel.Text == "Parar")
            {
                tempoInicial = tempoDefinido;
                tempoRestante = tempoInicial;

                // ✅ força a barra ao tamanho máximo imediatamente
                double larguraTotal = BarraContainer.Width;
                if (larguraTotal <= 0) larguraTotal = this.Width - 40;

                estadoTemporizador = EstadoTemporizador.Parado;

                BarraLaranja.WidthRequest = larguraTotal;
            }

            AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
            AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
            StatusLabel.Text = "TEMPO RESTANTE";

            _alarmePlayer?.Stop();

            timer.Stop();

            // ✅ Para a vibração em loop 
            var vibrator = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService); 
            vibrator?.Cancel();

            AtualizarUI();
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (tempoRestante.TotalSeconds == 0)
                return;

            if (estadoTemporizador == EstadoTemporizador.Parado || estadoTemporizador == EstadoTemporizador.Pausado || estadoTemporizador == EstadoTemporizador.Resetado)
            {
                timer.Start();
                estadoTemporizador = EstadoTemporizador.Rodando;
                AtualizarBotaoIniciar("Pausar", "pause.png", Colors.Orange);
                AtualizarBotaoReset("Parar", "stop.png", Colors.Red);

                // muda o texto do status
                StatusLabel.Text = "COZINHANDO...";
            }
            else if (estadoTemporizador == EstadoTemporizador.Rodando)
            {
                timer.Stop();
                estadoTemporizador = EstadoTemporizador.Pausado;
                AtualizarBotaoIniciar("Continuar", "play.png", Colors.Blue);
                AtualizarBotaoReset("Parar", "stop.png", Colors.Red);

                // volta para texto padrão
                StatusLabel.Text = "TEMPO RESTANTE";
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (estadoTemporizador == EstadoTemporizador.Rodando)
            {
                if (tempoRestante.TotalSeconds > 0)
                {
                    tempoRestante = tempoRestante.Subtract(TimeSpan.FromSeconds(1));

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        AtualizarUI();
                        AtualizarNotificacaoAndroid(tempoRestante.ToString("hh\\:mm\\:ss"));
                    });
                }
                else
                {
                    timer.Stop();
                    estadoTemporizador = EstadoTemporizador.Resetado;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TempoRestanteLabel.Text = "00:00:00";
                        BarraLaranja.WidthRequest = 0;
                        TempoRestanteLabel.Scale = 1;

                        // Toca o alarme
                        _alarmePlayer?.Play();

                        var vibrator = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService);

                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        {
                            // padrão: [início imediato, vibra 1000ms, pausa 1000ms]
                            long[] pattern = { 0, 1000, 1000 };
                            var effect = VibrationEffect.CreateWaveform(pattern, 0); // 0 = repetir indefinidamente
                            vibrator?.Vibrate(effect);
                        }
                        else
                        {
                            // versões antigas
                            long[] pattern = { 0, 1000, 1000 };
                            vibrator?.Vibrate(pattern, 0); // 0 = repetir indefinidamente
                        }

                        // Atualiza a notificação
                        AtualizarNotificacaoAndroid("Tempo concluído!");
                    });
                }

                return;
            }

            if (estadoTemporizador == EstadoTemporizador.Pausado)
                return;

            if (estadoTemporizador == EstadoTemporizador.Parado)
            {
                timer.Stop();
                tempoRestante = tempoDefinido;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AtualizarUI();
                    AtualizarNotificacaoAndroid(tempoRestante.ToString("hh\\:mm\\:ss"));
                });
                return;
            }

            if (estadoTemporizador == EstadoTemporizador.Resetado)
            {
                timer.Stop();
                tempoRestante = TimeSpan.Zero;
                tempoInicial = TimeSpan.Zero;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AtualizarUI();
                    AtualizarNotificacaoAndroid("00:00:00");
                    StopNotificationAndroid();

                    // Para a vibração quando resetar
                    var vibrator = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService);
                    vibrator?.Cancel();
                });
                return;
            }
        }

        private void OnReceitaSelecionada(object sender, EventArgs e)
        {
            if (sender is Frame frame)
            {
                if (frame.Content is Grid grid && grid.Children.Count >= 2)
                {
                    var tempoLabel = grid.Children[1] as Label;
                    if (tempoLabel != null)
                    {
                        string textoTempo = tempoLabel.Text;

                        if (TimeSpan.TryParse(textoTempo, out var tempo))
                        {
                            tempoDefinido = tempo;
                            tempoInicial = tempo;
                            tempoRestante = tempo;

                            estadoTemporizador = EstadoTemporizador.Parado;

                            AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
                            AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));

                            AtualizarUI();
                        }
                    }
                }
            }
        }

        private void AtualizarBotaoIniciar(string texto, string iconSource, Color cor)
        {
            BotaoIniciar.BackgroundColor = cor;
            BotaoIniciarLabel.Text = texto;
            BotaoIniciarIcon.Source = iconSource;
        }

        private void AtualizarBotaoReset(string texto, string iconSource, Color cor)
        {
            BotaoReset.BackgroundColor = cor;
            BotaoResetLabel.Text = texto;
            BotaoResetIcon.Source = iconSource;
        }

        private async void OnNovaReceitaClicked(object sender, EventArgs e)
        {
            var popup = new NovaReceitaPopup(); // ✅ passa a conexão para o popup
            var resultado = await this.ShowPopupAsync(popup);

            if (resultado is Receita receita)
            {
                tempoDefinido = TimeSpan.FromSeconds(receita.Tempo);
                tempoInicial = tempoDefinido;
                tempoRestante = tempoDefinido;

                // Atualiza lista de receitas
                CarregarReceitas();
                AtualizarUI();
            }
        }

        // Método para pausar via notificação (chamado pelo receiver)
        public void PausarTimer()
        {
            if (estadoTemporizador == EstadoTemporizador.Rodando)
            {
                timer.Stop();
                estadoTemporizador = EstadoTemporizador.Pausado;
                AtualizarBotaoIniciar("Continuar", "play.png", Colors.Blue);
                StatusLabel.Text = "TEMPO RESTANTE";
                AtualizarUI();
            }
        }

        // Método para continuar via notificação
        public void ContinuarTimer()
        {
            if (estadoTemporizador == EstadoTemporizador.Pausado)
            {
                timer.Start();
                estadoTemporizador = EstadoTemporizador.Rodando;
                AtualizarBotaoIniciar("Pausar", "pause.png", Colors.Orange);
                StatusLabel.Text = "COZINHANDO...";
                AtualizarUI();
            }
        }

        public class AtualizarNotificacaoMessage 
        { 
            public string NovoTempo { get; set; }
            public bool EstaRodando { get; set; }
        }

        // Método para parar via notificação
        public void PararTimer()
        {
            if (BotaoResetLabel.Text == "Parar")
            {
                tempoInicial = tempoDefinido;
                tempoRestante = tempoInicial;

                // ✅ força a barra ao tamanho máximo imediatamente
                double larguraTotal = BarraContainer.Width;
                if (larguraTotal <= 0) larguraTotal = this.Width - 40;

                estadoTemporizador = EstadoTemporizador.Parado;

                BarraLaranja.WidthRequest = larguraTotal;

                AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
                AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
                StatusLabel.Text = "TEMPO RESTANTE";

                _alarmePlayer?.Stop();

                timer.Stop();

                tempoRestante = tempoInicial; // Resetando o tempo
        
                // Envia comando para atualizar a notificação com o tempo resetado
                WeakReferenceMessenger.Default.Send(new AtualizarNotificacaoMessage 
                { 
                    NovoTempo = tempoRestante.ToString(@"mm\:ss"), 
                    EstaRodando = false 
                });

                AtualizarUI();
            }
        }

        // Método para resetar via notificação
        public void ResetarTimer()
        {
            tempoInicial = TimeSpan.Zero;
            tempoRestante = TimeSpan.Zero;
            estadoTemporizador = EstadoTemporizador.Resetado;

            AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
            AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
            StatusLabel.Text = "TEMPO RESTANTE";

            _alarmePlayer?.Stop();

            timer.Stop();

            AtualizarUI();
            StopNotificationAndroid();  // Removes notification on reset
        }
    }

    // BroadcastReceiver para ações da notificação
    [BroadcastReceiver(Enabled = true, Label = "Timer Receiver", Exported = true)]
    public class TimerBroadcastReceiver : BroadcastReceiver
    {
        private readonly MainPage _mainPage;

        public TimerBroadcastReceiver()
        {
        }

        public TimerBroadcastReceiver(MainPage mainPage)
        {
            _mainPage = mainPage;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (_mainPage == null) return;  // Segurança se não inicializado

            string action = intent.GetStringExtra("action");

            if (action == "PAUSAR")
            {
                _mainPage.PausarTimer();
            }
            else if (action == "CONTINUAR")
            {
                _mainPage.ContinuarTimer();
            }
            else if (action == "PARAR")
            {
                _mainPage.PararTimer();
            }
            else if (action == "RESETAR")
            {
                _mainPage.ResetarTimer();
            }
        }
    }
}