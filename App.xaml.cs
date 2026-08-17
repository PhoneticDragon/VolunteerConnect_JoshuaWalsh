namespace VolunteerConnect;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

        RunAutomatedTests();
    }

    private async void RunAutomatedTests()
    {
        var tests = new AutomatedTesting();

        await tests.RunAllTests();
    }
}