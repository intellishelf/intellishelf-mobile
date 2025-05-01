namespace Intellishelf;

public partial class App
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) {
        var token = Preferences.Get("JwtToken", null);

        var shell = new AppShell();

        shell.GoToAsync(string.IsNullOrEmpty(token) ? "//Login" : "//Books");

        return new Window(shell);
    }
}