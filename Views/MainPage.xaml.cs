using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using System;
using System.IO;
using System.Timers;
using Temporizador.Popups;
using Temporizador.Models;
using SQLite;
using Microsoft.Maui.Controls.Shapes;
using CommunityToolkit.Maui.Behaviors;
using Plugin.Maui.Audio;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;
#if ANDROID
using AndroidX.Core.Content;        // Para ContextCompat
#endif
#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Android.Widget;
#endif

namespace Temporizador.Views
{
    public partial class MainPage : ContentPage
    {
        private SQLiteConnection _db;
        private TimeSpan tempoInicial;
        private TimeSpan tempoRestante;
        private IDispatcherTimer timer;

        private enum EstadoTemporizador { Parado, Rodando, Pausado, Resetado }
        private EstadoTemporizador estadoTemporizador = EstadoTemporizador.Resetado;
        private TimeSpan tempoDefinido;

        private IAudioPlayer _alarmePlayer;
        private bool _audioInitialized = false;

#if ANDROID
        private long _endElapsedMillis;
        private const int MaxNotificationsPerMinute = 30; // Limite de segurança
        private DateTime _lastNotificationReset = DateTime.Now;
#endif

        public MainPage()
        {
            InitializeComponent();

            // Registro para mensagens do TimerService
            WeakReferenceMessenger.Default.Register<PararTimerPelaNotificacaoMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    PararTimer();
                });
            });

            WeakReferenceMessenger.Default.Register<ResetarTimerPelaNotificacaoMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ResetarTimer();
                });
            });

            WeakReferenceMessenger.Default.Register<IniciarTimerPelaNotificacaoMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    OnStartClicked(null, null);
                });
            });

            WeakReferenceMessenger.Default.Register<PausarTimerPelaNotificacaoMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (estadoTemporizador == EstadoTemporizador.Rodando)
                    {
                        PausarTimer();
                    }
                });
            });

            // Inicializa o banco de dados
            var dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "receitas.db");
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Receita>();

            tempoInicial = TimeSpan.Zero;
            tempoRestante = tempoInicial;
            tempoDefinido = TimeSpan.Zero;

            timer = this.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerElapsed;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (!_audioInitialized)
                {
                    await InitializeAudioPlayerAsync();
                    _audioInitialized = true;
                }

                CarregarReceitas();
                LoadTimerState();
                AtualizarUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no OnAppearing: {ex.Message}");
            }
        }

        // Handler para o evento Loaded definido em XAML
        private void OnPageLoaded(object sender, EventArgs e)
        {
            // Garante que a UI esteja sincronizada após o carregamento inicial
            AtualizarUI();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            if (timer != null && timer.IsRunning && estadoTemporizador == EstadoTemporizador.Rodando)
            {
                timer.Stop();
                System.Diagnostics.Debug.WriteLine("App desaparecendo - Timer local parado");
            }
        }

        private async Task InitializeAudioPlayerAsync()
        {
            try
            {
                var audioManager = AudioManager.Current;
                await using var stream = await FileSystem.OpenAppPackageFileAsync("alarme.mp3");
                _alarmePlayer = audioManager.CreatePlayer(stream);
                _alarmePlayer.Loop = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Falha ao carregar alarme: {ex.Message}");
            }
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
                    BindingContext = receita
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

                // TouchBehavior com toque normal e long press
                var touchBehavior = new TouchBehavior
                {
                    Command = new Command(() =>
                    {
                        OnReceitaSelecionada(frame, EventArgs.Empty);
                    }),
                    LongPressCommand = new Command(async () =>
                    {
                        var popup = new EditarReceitaPopup(receita, _db);
                        var resultado = await this.ShowPopupAsync(popup);
                        CarregarReceitas();
                    }),
                    LongPressDuration = 600,
                };
                frame.Behaviors.Add(touchBehavior);

                ReceitasContainer.Add(frame, col, row);

                col++;
                if (col > 1) { col = 0; row++; }
            }

            // Card "Nova Receita"
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

        private void AtualizarUI()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TempoRestanteLabel.Text = tempoRestante.ToString(@"hh\:mm\:ss");

                double larguraTotal = BarraContainer.Width;
                if (larguraTotal <= 0)
                    larguraTotal = this.Width - 40;

                double progresso = tempoInicial.TotalSeconds > 0
                    ? tempoRestante.TotalSeconds / tempoInicial.TotalSeconds
                    : 0;

                double novaLargura = larguraTotal * progresso;

                switch (estadoTemporizador)
                {
                    case EstadoTemporizador.Rodando:
                        BarraLaranja.WidthRequest = novaLargura;
                        break;
                    case EstadoTemporizador.Pausado:
                        // Mantém largura atual
                        break;
                    case EstadoTemporizador.Parado:
                        BarraLaranja.WidthRequest = larguraTotal;
                        break;
                    case EstadoTemporizador.Resetado:
                        BarraLaranja.WidthRequest = 0;
                        break;
                }
            });
        }

        private void OnAddTimeClicked(object sender, EventArgs e)
        {
            estadoTemporizador = EstadoTemporizador.Parado;

            if (sender is Border border &&
                border.GestureRecognizers[0] is TapGestureRecognizer tap &&
                tap.CommandParameter is string s &&
                int.TryParse(s, out int addSeg))
            {
#if ANDROID
                if (estadoTemporizador == EstadoTemporizador.Rodando)
                {
                    _endElapsedMillis += addSeg * 1000;
                }
                else
#endif
                {
                    tempoRestante = tempoRestante.Add(TimeSpan.FromSeconds(addSeg));
                }
                tempoInicial = tempoInicial.Add(TimeSpan.FromSeconds(addSeg));
                tempoDefinido = tempoInicial;
                AtualizarUI();
                SaveTimerState();
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            bool ehReset = BotaoResetLabel.Text == "Reset";
            bool ehParar = BotaoResetLabel.Text == "Parar";

            timer.Stop();
            _alarmePlayer?.Stop();

#if ANDROID
            var vibrator = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService);
            vibrator?.Cancel();
#endif

            if (ehReset)
            {
                tempoInicial = TimeSpan.Zero;
                tempoRestante = TimeSpan.Zero;
                tempoDefinido = TimeSpan.Zero;
                estadoTemporizador = EstadoTemporizador.Resetado;
#if ANDROID
                _endElapsedMillis = 0;
#endif
                StopTimerService();
            }
            else if (ehParar)
            {
                tempoInicial = tempoDefinido;
                tempoRestante = tempoInicial;
#if ANDROID
                _endElapsedMillis = 0;
#endif
                double larguraTotal = BarraContainer.Width;
                if (larguraTotal <= 0) larguraTotal = this.Width - 40;

                estadoTemporizador = EstadoTemporizador.Parado;
                BarraLaranja.WidthRequest = larguraTotal;

                // Envia mensagem para parar
#if ANDROID
                WeakReferenceMessenger.Default.Send(new PararTimerPelaNotificacaoMessage());
#endif
            }

            AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
            AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
            StatusLabel.Text = "TEMPO RESTANTE";

            AtualizarUI();
            SaveTimerState();
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            if (tempoRestante.TotalSeconds == 0)
                return;

            if (estadoTemporizador == EstadoTemporizador.Parado || 
                estadoTemporizador == EstadoTemporizador.Resetado)
            {
#if ANDROID
                _endElapsedMillis = Android.OS.SystemClock.ElapsedRealtime() + (long)tempoRestante.TotalMilliseconds;
#endif
                timer.Start();
                estadoTemporizador = EstadoTemporizador.Rodando;
                AtualizarBotaoIniciar("Pausar", "pause.png", Colors.Orange);
                AtualizarBotaoReset("Parar", "stop.png", Colors.Red);
                StatusLabel.Text = "COZINHANDO...";

                StartTimerService(true);
            }
            else if (estadoTemporizador == EstadoTemporizador.Pausado)
            {
#if ANDROID
                _endElapsedMillis = Android.OS.SystemClock.ElapsedRealtime() + (long)tempoRestante.TotalMilliseconds;
#endif
                timer.Start();
                estadoTemporizador = EstadoTemporizador.Rodando;
                AtualizarBotaoIniciar("Pausar", "pause.png", Colors.Orange);
                AtualizarBotaoReset("Parar", "stop.png", Colors.Red);
                StatusLabel.Text = "COZINHANDO...";

                StartTimerService(true);
            }
            else if (estadoTemporizador == EstadoTemporizador.Rodando)
            {
                PausarTimer();
            }
            SaveTimerState();
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            if (estadoTemporizador == EstadoTemporizador.Rodando)
            {
#if ANDROID
                long remainingMillis = _endElapsedMillis - Android.OS.SystemClock.ElapsedRealtime();
                if (remainingMillis > 0)
                {
                    tempoRestante = TimeSpan.FromMilliseconds(remainingMillis);
                    AtualizarUI();
                }
                else
                {
                    TimerExpired();
                }
#endif
            }
        }

        private void TimerExpired()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                timer.Stop();
                estadoTemporizador = EstadoTemporizador.Resetado;
#if ANDROID
                _endElapsedMillis = 0;
#endif
                tempoRestante = TimeSpan.Zero;
                TempoRestanteLabel.Text = "00:00:00";
                BarraLaranja.WidthRequest = 0;

                // Toca alarme
                _alarmePlayer?.Play();

                // Vibração
#if ANDROID
                var vibrator = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    long[] pattern = { 0, 500, 300, 500, 300, 500 };
                    var effect = VibrationEffect.CreateWaveform(pattern, -1);
                    vibrator?.Vibrate(effect);
                }
                else
                {
#pragma warning disable CS0618
                    long[] pattern = { 0, 500, 300, 500, 300, 500 };
                    vibrator?.Vibrate(pattern, -1);
#pragma warning restore CS0618
                }
#endif

                SaveTimerState();
            });
        }

        private void OnReceitaSelecionada(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.Content is Grid grid && grid.Children.Count >= 2)
            {
                if (grid.Children[1] is Label tempoLabel)
                {
                    if (TimeSpan.TryParse(tempoLabel.Text, out var tempo))
                    {
                        tempoDefinido = tempo;
                        tempoInicial = tempo;
                        tempoRestante = tempo;
                        estadoTemporizador = EstadoTemporizador.Parado;

                        AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
                        AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));

                        AtualizarUI();
                        SaveTimerState();
                    }
                }
            }
        }

        private void AtualizarBotaoIniciar(string texto, string iconSource, Color cor)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                BotaoIniciar.BackgroundColor = cor;
                BotaoIniciarLabel.Text = texto;
                BotaoIniciarIcon.Source = iconSource;
            });
        }

        private void AtualizarBotaoReset(string texto, string iconSource, Color cor)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                BotaoReset.BackgroundColor = cor;
                BotaoResetLabel.Text = texto;
                BotaoResetIcon.Source = iconSource;
            });
        }

        private async void OnNovaReceitaClicked(object sender, EventArgs e)
        {
            var popup = new NovaReceitaPopup();
            var resultado = await this.ShowPopupAsync(popup);  // Agora deve funcionar

            if (resultado is Receita receita)
            {
                tempoDefinido = TimeSpan.FromSeconds(receita.Tempo);
                tempoInicial = tempoDefinido;
                tempoRestante = tempoDefinido;

                CarregarReceitas();
                AtualizarUI();
                SaveTimerState();
            }
        }

        public void PausarTimer()
        {
            if (estadoTemporizador == EstadoTemporizador.Rodando)
            {
#if ANDROID
                long remainingMillis = _endElapsedMillis - Android.OS.SystemClock.ElapsedRealtime();
                if (remainingMillis > 0)
                {
                    tempoRestante = TimeSpan.FromMilliseconds(remainingMillis);
                }
                _endElapsedMillis = 0;
#endif
                timer.Stop();
                estadoTemporizador = EstadoTemporizador.Pausado;
                AtualizarBotaoIniciar("Continuar", "play.png", Colors.Blue);
                AtualizarBotaoReset("Parar", "stop.png", Colors.Red);
                StatusLabel.Text = "TEMPO RESTANTE";
                AtualizarUI();
                SaveTimerState();
            }
        }

        public void PararTimer()
        {
            if (BotaoResetLabel.Text == "Parar")
            {
#if ANDROID
                _endElapsedMillis = 0;
#endif
                tempoInicial = tempoDefinido;
                tempoRestante = tempoInicial;

                double larguraTotal = BarraContainer.Width;
                if (larguraTotal <= 0) larguraTotal = this.Width - 40;

                estadoTemporizador = EstadoTemporizador.Parado;
                BarraLaranja.WidthRequest = larguraTotal;

                AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
                AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
                StatusLabel.Text = "TEMPO RESTANTE";

                _alarmePlayer?.Stop();

#if ANDROID
                var vibrator = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService);
                vibrator?.Cancel();
#endif

                timer.Stop();
                AtualizarUI();
                SaveTimerState();
            }
        }

        public void ResetarTimer()
        {
#if ANDROID
            _endElapsedMillis = 0;
#endif
            tempoInicial = TimeSpan.Zero;
            tempoRestante = TimeSpan.Zero;
            tempoDefinido = TimeSpan.Zero;
            estadoTemporizador = EstadoTemporizador.Resetado;

            AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
            AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
            StatusLabel.Text = "TEMPO RESTANTE";

            _alarmePlayer?.Stop();
            timer.Stop();

            AtualizarUI();
            StopTimerService();
            SaveTimerState();
        }

        private void StartTimerService(bool rodando)
        {
#if ANDROID
            try
            {
                var intent = new Intent(Android.App.Application.Context, typeof(TimerService));
                intent.PutExtra("tempo", tempoRestante.ToString(@"hh\:mm\:ss"));
                intent.PutExtra("tempoInicial", tempoDefinido.ToString(@"hh\:mm\:ss"));
                intent.PutExtra("estaRodando", rodando);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    ContextCompat.StartForegroundService(Android.App.Application.Context, intent);
                else
                    Android.App.Application.Context.StartService(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao iniciar serviço: {ex.Message}");
            }
#endif
        }

        private void StopTimerService()
        {
#if ANDROID
            try
            {
                var intent = new Intent(Android.App.Application.Context, typeof(TimerService));
                Android.App.Application.Context.StopService(intent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parar serviço: {ex.Message}");
            }
#endif
        }

        private void SaveTimerState()
        {
            try
            {
                Preferences.Default.Set("estaRodando", estadoTemporizador == EstadoTemporizador.Rodando);
                Preferences.Default.Set("tempoInicial", tempoInicial.ToString(@"hh\:mm\:ss"));
                Preferences.Default.Set("tempoDefinido", tempoDefinido.ToString(@"hh\:mm\:ss"));
                
                if (estadoTemporizador == EstadoTemporizador.Rodando)
                {
#if ANDROID
                    Preferences.Default.Set("endElapsedMillis", _endElapsedMillis);
#endif
                }
                else
                {
                    Preferences.Default.Set("endElapsedMillis", 0L);
                    Preferences.Default.Set("tempoRestante", tempoRestante.ToString(@"hh\:mm\:ss"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar estado: {ex.Message}");
            }
        }

        private void LoadTimerState()
        {
            try
            {
                var rodando = Preferences.Default.Get("estaRodando", false);
                TimeSpan.TryParse(Preferences.Default.Get("tempoInicial", "00:00:00"), out tempoInicial);
                TimeSpan.TryParse(Preferences.Default.Get("tempoDefinido", "00:00:00"), out tempoDefinido);

                if (rodando)
                {
#if ANDROID
                    _endElapsedMillis = Preferences.Default.Get("endElapsedMillis", 0L);
                    long remainingMillis = _endElapsedMillis - Android.OS.SystemClock.ElapsedRealtime();

                    if (remainingMillis > 0)
                    {
                        tempoRestante = TimeSpan.FromMilliseconds(remainingMillis);
                        estadoTemporizador = EstadoTemporizador.Rodando;
                        
                        if (!timer.IsRunning)
                        {
                            timer.Start();
                        }
                        
                        AtualizarBotaoIniciar("Pausar", "pause.png", Colors.Orange);
                        AtualizarBotaoReset("Parar", "stop.png", Colors.Red);
                        StatusLabel.Text = "COZINHANDO...";
                    }
                    else
                    {
                        TimerExpired();
                    }
#endif
                }
                else
                {
                    TimeSpan.TryParse(Preferences.Default.Get("tempoRestante", "00:00:00"), out tempoRestante);
                    estadoTemporizador = EstadoTemporizador.Parado;
                    AtualizarBotaoIniciar("Iniciar", "play.png", Color.FromArgb("#27AE60"));
                    AtualizarBotaoReset("Reset", "reset.png", Color.FromArgb("#E74C3C"));
                    StatusLabel.Text = "TEMPO RESTANTE";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar estado: {ex.Message}");
                // Estado padrão em caso de erro
                tempoRestante = TimeSpan.Zero;
                tempoInicial = TimeSpan.Zero;
                tempoDefinido = TimeSpan.Zero;
                estadoTemporizador = EstadoTemporizador.Resetado;
            }
        }
    }

    // Mensagens para comunicação entre MainPage e TimerService
    public class PararTimerPelaNotificacaoMessage { }
    public class ResetarTimerPelaNotificacaoMessage { }
    public class IniciarTimerPelaNotificacaoMessage { }
    public class PausarTimerPelaNotificacaoMessage { }
}