using CommunityToolkit.Maui.Views;
using SQLite;
using Temporizador.Models;

namespace Temporizador.Popups
{
    public partial class EditarReceitaPopup : Popup
    {
        private readonly SQLiteConnection _db;
        private readonly Receita _receita;

        public EditarReceitaPopup(Receita receita, SQLiteConnection db)
        {
            InitializeComponent();
            _db = db;
            _receita = receita;

            // Preenche os campos com os dados atuais
            NomeReceitaEntry.Text = receita.Nome;

            var tempo = TimeSpan.FromSeconds(receita.Tempo);
            HorasEntry.Text = tempo.Hours.ToString();
            MinutosEntry.Text = tempo.Minutes.ToString();
            SegundosEntry.Text = tempo.Seconds.ToString();

            // Seleciona a cor atual (exibe checkmark)
            SelecionarCor(receita.Cor);
        }

        private async void OnFecharClicked(object sender, EventArgs e) => await CloseAsync();

        private void SelecionarCor(string corHex)
        {
            // Oculta todos os checkmarks
            Cor1Check.IsVisible = Cor2Check.IsVisible = Cor3Check.IsVisible =
            Cor4Check.IsVisible = Cor5Check.IsVisible = Cor6Check.IsVisible = false;

            // Mostra apenas o checkmark da cor atual
            if (corHex == "#FF6B35") Cor1Check.IsVisible = true;
            else if (corHex == "#27AE60") Cor2Check.IsVisible = true;
            else if (corHex == "#E74C3C") Cor3Check.IsVisible = true;
            else if (corHex == "#FFD60A") Cor4Check.IsVisible = true;
            else if (corHex == "#3498DB") Cor5Check.IsVisible = true;
            else if (corHex == "#9B59B6") Cor6Check.IsVisible = true;
        }

        private async void OnSalvarEdicaoClicked(object sender, EventArgs e)
        {
            try
            {
                // Validação básica
                if (string.IsNullOrWhiteSpace(NomeReceitaEntry.Text))
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Digite um nome para a receita.", "OK");
                    return;
                }

                int horas = int.TryParse(HorasEntry.Text, out var h) ? h : 0;
                int minutos = int.TryParse(MinutosEntry.Text, out var m) ? m : 0;
                int segundos = int.TryParse(SegundosEntry.Text, out var s) ? s : 0;

                var tempoTotal = new TimeSpan(horas, minutos, segundos);
                if (tempoTotal.TotalSeconds <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Defina um tempo válido.", "OK");
                    return;
                }

                // Atualiza os dados
                _receita.Nome = NomeReceitaEntry.Text.Trim();
                _receita.Tempo = (int)tempoTotal.TotalSeconds;

                // Cor selecionada
                if (Cor1Check.IsVisible) _receita.Cor = "#FF6B35";
                else if (Cor2Check.IsVisible) _receita.Cor = "#27AE60";
                else if (Cor3Check.IsVisible) _receita.Cor = "#E74C3C";
                else if (Cor4Check.IsVisible) _receita.Cor = "#FFD60A";
                else if (Cor5Check.IsVisible) _receita.Cor = "#3498DB";
                else if (Cor6Check.IsVisible) _receita.Cor = "#9B59B6";

                _db.Update(_receita);

                // Agora mostra o alerta (fora do popup)
                await Application.Current.MainPage.DisplayAlert("Sucesso", "Receita atualizada com sucesso!", "OK");

                // Fecha ANTES de mostrar o alert
                await CloseAsync(); // opcional: retorna a receita atualizada
             
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível salvar: " + ex.Message, "OK");
            }
        }

        private async void OnDeletarReceitaClicked(object sender, EventArgs e)
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert("Confirmar", "Deseja realmente deletar esta receita?", "Sim", "Não");
            
            if (confirmar)
            {
                try
                {
                    _db.Delete(_receita);

                    // Fecha primeiro
                    await CloseAsync(); // ou CloseAsync(true) se quiser sinalizar exclusão

                    await Application.Current.MainPage.DisplayAlert("Sucesso", "Receita deletada com sucesso!", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível deletar: " + ex.Message, "OK");
                }
            }
        }

        private void OnCorSelecionada(object sender, EventArgs e)
        {
            // Oculta todos os checkmarks
            Cor1Check.IsVisible = false;
            Cor2Check.IsVisible = false;
            Cor3Check.IsVisible = false;
            Cor4Check.IsVisible = false;
            Cor5Check.IsVisible = false;
            Cor6Check.IsVisible = false;

            // Mostra apenas o checkmark da cor clicada
            if (sender == Cor1Button) Cor1Check.IsVisible = true;
            else if (sender == Cor2Button) Cor2Check.IsVisible = true;
            else if (sender == Cor3Button) Cor3Check.IsVisible = true;
            else if (sender == Cor4Button) Cor4Check.IsVisible = true;
            else if (sender == Cor5Button) Cor5Check.IsVisible = true;
            else if (sender == Cor6Button) Cor6Check.IsVisible = true;
        }
    }
}
