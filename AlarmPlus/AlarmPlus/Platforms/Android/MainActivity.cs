using AlarmPlus.Platforms.Android;
using Android.App;
using Android.Content;
using Android.Content.PM;

namespace AlarmPlus;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public MainActivity()
    {
        AndroidServiceManager.MainActivity = this;
    }
    public void StartService(string srs)
    {
        var serviceIntent = new Intent(this, typeof(MyBackgroundService));
        serviceIntent.PutExtra("inputExtra", "Alarm Plus Service");
        serviceIntent.PutExtra("remindersSZ", srs);

        // serialize
        //Bundle bundle = new Bundle();
        //bundle.PutSerializable("rsSerialized",srs);
        //serviceIntent.PutExtra("bundlersSer", bundle);
        StartService(serviceIntent);
    }

    public void StopService()
    {
        var serviceIntent = new Intent(this, typeof(MyBackgroundService));
        StopService(serviceIntent);
    }

  
}

