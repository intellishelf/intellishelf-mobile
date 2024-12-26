namespace Intellishelf;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        string token = Preferences.Get("JwtToken", null);
        if (string.IsNullOrEmpty(token))
        {
            MainPage = new NavigationPage(new LoginPage());
        }
        else
        {
            MainPage = new NavigationPage(new MainPage());
        }
    }
}