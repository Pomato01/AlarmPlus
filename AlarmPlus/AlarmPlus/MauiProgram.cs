using Microsoft.Extensions.Logging;
using AlarmPlus.Data;
using AlarmPlus.Views;
using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;

namespace AlarmPlus;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkitMediaElement();
        builder.Services.AddSingleton<Reminders>();
        builder.Services.AddTransient<Reminder>();
        builder.Services.AddTransient<Alarm>();
        builder.Services.AddTransient<AlarmPlusDatabase>();
        // for android life cycel events
/*#if ANDROID
        builder.ConfigureLifecycleEvents(events =>
        events.AddAndroid(android =>
        android
                        .OnStart((activity) => LogEvent(nameof(AndroidLifecycle.OnStart)))
                        .OnCreate((activity, bundle) => LogEvent(nameof(AndroidLifecycle.OnCreate)))
                        .OnBackPressed((activity) => LogEvent(nameof(AndroidLifecycle.OnBackPressed)) && false)
                        .OnStop((activity) => LogEvent(nameof(AndroidLifecycle.OnStop)))
                        .OnPause((activity)=> LogEvent(nameof(AndroidLifecycle.OnPause)))
                        ));
#endif*/
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }

    static bool LogEvent(string eventName, string type = null)
    {
        System.Diagnostics.Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");
        return true;
    }
}