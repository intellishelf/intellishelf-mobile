using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Intellishelf.Clients;
using Intellishelf.Models.Auth;
using Intellishelf.Services;
using Intellishelf.Services.Implementation;

namespace Intellishelf.Pages;

public partial class Login
{
    private readonly IIntellishelfAuthClient _authClient;
    private readonly IAuthStorage _tokenService;

    public Login(IIntellishelfAuthClient authClient, IAuthStorage tokenService)
    {
        _authClient = authClient;
        _tokenService = tokenService;
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            UiNotifier.ShowError("Please enter both email and password");
            return;
        }

        var result = await _authClient.LoginAsync(new UserCredentials(email, password));

        if (result.IsSuccess)
        {
            _tokenService.StoreToken(result.Value);
            
            EmailEntry.Text = "";
            PasswordEntry.Text = "";
            
            await Shell.Current.GoToAsync("//Books");
        }
        else
        {
            UiNotifier.ShowError(result.Error.Message);
        }
    }
}