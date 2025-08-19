using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Intellishelf.Clients;
using Intellishelf.Models.Auth;
using Intellishelf.Services;

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
            ShowError("Please enter both email and password");
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
            ShowError(result.Error.Message);
        }
    }

    private static void ShowError(string message)
    {
        var resources = Application.Current.Resources;

        Snackbar.Make(message, visualOptions: new SnackbarOptions
        {
            BackgroundColor       = (Color)resources["Magenta"],
            TextColor             = (Color)resources["White"],
            ActionButtonTextColor = (Color)resources["White"]
        }).Show();
    }
}