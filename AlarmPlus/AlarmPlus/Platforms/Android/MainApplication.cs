using Android.App;
using Android.OS;
using Android.Runtime;

namespace AlarmPlus;

[Application]
public class MainApplication : MauiApplication
{
    public static readonly string ChannelId = "backgroundServiceChannel";
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void OnCreate()
    {
        base.OnCreate();
        if(Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
#pragma warning disable CA1416
            var serviceChnnel =
                new NotificationChannel(ChannelId, "Alarm Plus Channel",
                NotificationImportance.High);
            if (GetSystemService(NotificationService) is NotificationManager manager)
            {
                manager.CreateNotificationChannel(serviceChnnel);
            }
#pragma warning restore CA1416
        }
    }
}

