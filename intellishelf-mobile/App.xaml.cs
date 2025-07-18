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

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell(_tokenService);

        shell.GoToAsync(_tokenService.IsAccessTokenValid() ? "//Login" : "//Books");

        return new Window(shell);
    }
}