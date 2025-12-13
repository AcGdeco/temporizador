using Temporizador.Views;

namespace Temporizador
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Views.MainPage());
        }
        
        protected override void OnStart()
        {
            base.OnStart();

            bool criarAtalho = false;   
            if (!Preferences.Get("shortcut_created", false) && criarAtalho == true)
            {
                var shortcutService = ServiceHelper.GetService<IShortcutService>();
                shortcutService.CreateShortcut();
                Preferences.Set("shortcut_created", true);
            }
        }
    }
}
