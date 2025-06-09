using Intellishelf.Services;

namespace Intellishelf;

public partial class App
{
    private readonly IAuthStorage _tokenService;
    public App(IAuthStorage tokenService)
    {
        _tokenService = tokenService;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) {
        var token = _tokenService.GetValidAccessToken();

        var shell = new AppShell();

        shell.GoToAsync(string.IsNullOrEmpty(token) ? "//Login" : "//Books");

        return new Window(shell);
    }
}