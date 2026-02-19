using Android.App;
using Android.Content;
using Android.OS;

namespace Temporizador.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new[] { "com.companyname.temporizador.ACTION_TIMER_FINISHED" })]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent == null)
                    return;

                var svc = new Intent(context, typeof(TimerService));
                svc.SetAction(intent.Action);
                svc.PutExtra("alarmFired", true);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    context.StartForegroundService(svc);
                else
                    context.StartService(svc);
            }
            catch
            {
                // Fail silently; TimerService will log errors
            }
        }
    }
}