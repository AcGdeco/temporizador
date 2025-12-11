using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Graphics.Drawables;
using Application = Android.App.Application;
using Temporizador;

public class ShortcutService : IShortcutService
{
    public void CreateShortcut()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var shortcutManager = (ShortcutManager)Application.Context.GetSystemService(Context.ShortcutService);

            if (shortcutManager != null && shortcutManager.IsRequestPinShortcutSupported)
            {
                var intent = new Intent(Application.Context, typeof(MainActivity));
                intent.SetAction(Intent.ActionMain);

                var shortcut = new ShortcutInfo.Builder(Application.Context, "temporizador_shortcut")
                    .SetShortLabel("Temporizador")
                    .SetLongLabel("Abrir Temporizador")
                    .SetIcon(Icon.CreateWithResource(Application.Context, Temporizador.Resource.Drawable.shortcut_icon))
                    .SetIntent(intent)
                    .Build();

                shortcutManager.RequestPinShortcut(shortcut, null);
            }
        }
    }
}
