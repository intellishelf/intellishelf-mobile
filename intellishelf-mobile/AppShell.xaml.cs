using Intellishelf.Services;

namespace Intellishelf;

public partial class AppShell
{
    private readonly IAuthStorage _authStorage;

    public AppShell(IAuthStorage authStorage)
    {
        InitializeComponent();
        _authStorage = authStorage;
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        _authStorage.ClearTokens();
        await Shell.Current.GoToAsync("//Login");
    }
}