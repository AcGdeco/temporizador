using System;
using CommunityToolkit.Maui.Views;
using Temporizador.Models;
using SQLite;

namespace Temporizador.Popups
{
    public partial class NovaReceitaPopup : Popup
    {
        private readonly SQLiteConnection _db;

        public NovaReceitaPopup()
        {
            InitializeComponent();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "receitas.db");
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Receita>();
        }

        private void OnFecharClicked(object sender, EventArgs e)
        {
            CloseAsync();
        }

        private void OnCorSelecionada(object sender, EventArgs e)
        {
            // Oculta todas as checkmarks
            Cor1Check.IsVisible = false;
            Cor2Check.IsVisible = false;
            Cor3Check.IsVisible = false;
            Cor4Check.IsVisible = false;
            Cor5Check.IsVisible = false;
            Cor6Check.IsVisible = false;

            // Mostra apenas a checkmark do botão clicado
            if (sender == Cor1Button) Cor1Check.IsVisible = true;
            else if (sender == Cor2Button) Cor2Check.IsVisible = true;
            else if (sender == Cor3Button) Cor3Check.IsVisible = true;
            else if (sender == Cor4Button) Cor4Check.IsVisible = true;
            else if (sender == Cor5Button) Cor5Check.IsVisible = true;
            else if (sender == Cor6Button) Cor6Check.IsVisible = true;
        }

        private async void OnSalvarReceitaClicked(object sender, EventArgs e)
        {
            // 1. Nome da receita
            string nome = NomeReceitaEntry?.Text?.Trim();
            if (string.IsNullOrEmpty(nome))
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "O nome da receita deve ser preenchido.", "OK");
                return;
            }

            // 2. Cor selecionada
            string corSelecionada = null;
            if (Cor1Check.IsVisible) corSelecionada = "#FF6B35";
            else if (Cor2Check.IsVisible) corSelecionada = "#27AE60";
            else if (Cor3Check.IsVisible) corSelecionada = "#E74C3C";
            else if (Cor4Check.IsVisible) corSelecionada = "#FFD60A";
            else if (Cor5Check.IsVisible) corSelecionada = "#3498DB";
            else if (Cor6Check.IsVisible) corSelecionada = "#9B59B6";

            if (corSelecionada == null)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Selecione uma cor para o botão.", "OK");
                return;
            }

            // 3. Tempo
            int horas = int.TryParse(HorasEntry?.Text, out var h) ? h : 0;
            int minutos = int.TryParse(MinutosEntry?.Text, out var m) ? m : 0;
            int segundos = int.TryParse(SegundosEntry?.Text, out var s) ? s : 0;

            if (horas == 0 && minutos == 0 && segundos == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Informe pelo menos horas, minutos ou segundos.", "OK");
                return;
            }

            horas = Math.Clamp(horas, 0, 999);
            minutos = Math.Clamp(minutos, 0, 59);
            segundos = Math.Clamp(segundos, 0, 59);

            var tempoTotal = new TimeSpan(horas, minutos, segundos);
            int tempoEmSegundos = (int)tempoTotal.TotalSeconds;

            // 4. Salvar no banco
            var receita = new Receita
            {
                Nome = nome,
                Tempo = tempoEmSegundos,
                Cor = corSelecionada
            };

            _db.Insert(receita);

            await Application.Current.MainPage.DisplayAlert("Sucesso", "Receita salva com sucesso!", "OK");

            // Fecha popup retornando a receita salva
            CloseAsync();
        }
    }
}
