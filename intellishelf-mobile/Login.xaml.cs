using Intellishelf.Models;
using Intellishelf.Services;

namespace Intellishelf;

public partial class Login
{
    private readonly IAuthService _authService;

    public Login(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Username and password are required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        var token = await _authService.LoginAsync(new UserCredentials(username, password));
        if (!string.IsNullOrWhiteSpace(token))
        {
            _authService.StoreToken(token);
            await Shell.Current.GoToAsync("//Books");
        }
        else
        {
            ErrorLabel.Text = "Invalid credentials.";
            ErrorLabel.IsVisible = true;
        }
    }
}