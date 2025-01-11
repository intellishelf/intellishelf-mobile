using Intellishelf.Clients;
using Intellishelf.Models;

namespace Intellishelf.Pages
{
    public partial class LoginPage
    {
        private readonly IIntellishelfApiClient _client;

        public LoginPage(IIntellishelfApiClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            // Basic validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorLabel.Text = "Username and password are required.";
                ErrorLabel.IsVisible = true;
                return;
            }

            var token = await _client.GetTokenAsync(new UserCredentials(username, password));
            if (!string.IsNullOrWhiteSpace(token))
            {
                // Store token and navigate to the main page
                Preferences.Set("JwtToken", token);
                await Shell.Current.GoToAsync("//Books");
            }
            else
            {
                ErrorLabel.Text = "Invalid credentials.";
                ErrorLabel.IsVisible = true;
            }
        }
    }
}