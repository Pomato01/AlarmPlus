namespace AlarmPlus;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

   /* protected override Window CreateWindow(IActivationState activationState)
    {
        Window window =  base.CreateWindow(activationState);
        window.Created += Window_Created;
        return window;
		
    }

    private void Window_Created(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }*/
}

