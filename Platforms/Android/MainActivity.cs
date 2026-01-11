using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App; // Para NotificationCompat
using Android; // Para Manifest
using AndroidX.Core.Content; // Necessário para ContextCompat

namespace Temporizador
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Corrigido: lógica de permissão deve estar dentro de um método, não diretamente na classe
            if ((int)Build.VERSION.SdkInt >= 33) // API 33+ (Tiramisu)
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.PostNotifications }, 1);
                }
            }
        }
    }
}
