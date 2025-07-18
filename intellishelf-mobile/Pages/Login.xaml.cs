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
            ErrorLabel.IsVisible = true;
            return;
        }

        var token = await _authClient.LoginAsync(new UserCredentials(email, password));

        _tokenService.StoreToken(token);

        ErrorLabel.IsVisible = false;
        EmailEntry.Text = "";
        PasswordEntry.Text = "";

        await Shell.Current.GoToAsync("//Books");
    }
}