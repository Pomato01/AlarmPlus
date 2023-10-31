namespace AlarmPlus;
using AlarmPlus.Views;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(Reminder), typeof(Reminder));
        Routing.RegisterRoute(nameof(Alarm), typeof(Alarm));
        Routing.RegisterRoute(nameof(About), typeof(About));
    }
}

