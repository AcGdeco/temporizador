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

namespace Temporizador.Views
{
    public partial class MainPage : ContentPage
    {
        private SQLiteConnection _db;   // ✅ conexão com o banco

        private TimeSpan tempoInicial;
        private TimeSpan tempoRestante;
        private System.Timers.Timer timer;

        private enum EstadoTemporizador { Parado, Rodando, Pausado }
        private EstadoTemporizador estadoTemporizador = EstadoTemporizador.Parado;
        private TimeSpan tempoDefinido;

        private IAudioPlayer _alarmePlayer;

        public MainPage()
        {
            InitializeComponent();

            // ✅ inicializa o banco
            var dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "receitas.db");
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Receita>();

            tempoInicial = TimeSpan.Zero;
            tempoRestante = tempoInicial;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;

            // ✅ Pré-carrega o áudio
            var audioManager = AudioManager.Current;
            using var stream = FileSystem.OpenAppPackageFileAsync("alarme.mp3").Result;
            _alarmePlayer = audioManager.CreatePlayer(stream);

            // 🔁 ativa loop automático
            _alarmePlayer.Loop = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarReceitas();   // ✅ carrega os cards do banco
            AtualizarUI();
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

            // Chama a animação do texto
            AnimarTempoRestanteLabel();

            // Pega a largura real do container
            double larguraTotal = BarraContainer.Width;
            if (larguraTotal <= 0) larguraTotal = this.Width - 40;

            double progresso = tempoRestante.TotalSeconds / Math.Max(1, tempoInicial.TotalSeconds);
            double novaLargura = larguraTotal * progresso;

            // Só anima se o botão iniciar foi clicado (temporizador rodando)
            if (estadoTemporizador == EstadoTemporizador.Rodando && tempoRestante.TotalSeconds != 0)
            {

                if(tempoRestante.TotalSeconds == 0)
                {
                    progresso = 0;
                }
                else {
                    progresso = (tempoRestante.TotalSeconds - 1) / Math.Max(1, tempoInicial.TotalSeconds);
                }
                novaLargura = larguraTotal * progresso;

                BarraLaranja.Animate(
                    "Progresso",
                    x => BarraLaranja.WidthRequest = x,
                    BarraLaranja.WidthRequest,
                    novaLargura,
                    length: 1000,
                    easing: Easing.Linear
                );
            }
            else
            {
                // Se não está rodando, apenas atualiza sem animar
                BarraLaranja.WidthRequest = novaLargura;
            }
        }

        private void OnAddTimeClicked(object sender, EventArgs e)
        {
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
            timer.Stop();

            if (BotaoResetLabel.Text == "Reset")
            {
                tempoInicial = TimeSpan.Zero;
                tempoRestante = TimeSpan.Zero;
            }
            else if (BotaoResetLabel.Text == "Parar")
            {
                tempoInicial = tempoDefinido;
                tempoRestante = tempoInicial;
            }

            estadoTemporizador = EstadoTemporizador.Parado;

            AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
            AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
            // volta para texto padrão
            StatusLabel.Text = "TEMPO RESTANTE";

            // ⏹️ para o som
            _alarmePlayer?.Stop();

            AtualizarUI();
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (tempoRestante.TotalSeconds == 0)
                return;

            if (estadoTemporizador == EstadoTemporizador.Parado || estadoTemporizador == EstadoTemporizador.Pausado)
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
            if (tempoRestante.TotalSeconds > 0)
            {
                tempoRestante = tempoRestante.Subtract(TimeSpan.FromSeconds(1));
                MainThread.BeginInvokeOnMainThread(AtualizarUI);
            }
            else
            {
                timer.Stop();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TempoRestanteLabel.Text = "00:00:00";
                    BarraLaranja.WidthRequest = 0;
                    TempoRestanteLabel.Scale = 1;

                    _alarmePlayer?.Play();
                });
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
    }
}
