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

    private async void OnBooksClicked(object sender, EventArgs e)
    {
        await Current.GoToAsync("//Books");
        Current.FlyoutIsPresented = false;
    }

    private async void OnAddBookClicked(object sender, EventArgs e)
    {
        await Current.GoToAsync("//AddBook");
        Current.FlyoutIsPresented = false;
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Current.GoToAsync("//Settings");
        Current.FlyoutIsPresented = false;
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        _authStorage.ClearTokens();
        await Current.GoToAsync("//Login");
        Current.FlyoutIsPresented = false;
    }
}