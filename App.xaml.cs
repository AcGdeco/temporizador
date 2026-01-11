using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Temporizador.Views;

namespace Temporizador
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Define a página inicial com NavigationPage (para popups funcionarem bem)
            MainPage = new NavigationPage(new Views.MainPage());
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Cria o atalho na tela inicial (Android) apenas na primeira vez que o app é aberto
            if (!Preferences.Get("shortcut_created", false))
            {
                var shortcutService = ServiceHelper.GetService<IShortcutService>();
                shortcutService?.CreateShortcut();
                Preferences.Set("shortcut_created", true);
            }
        }
    }
}