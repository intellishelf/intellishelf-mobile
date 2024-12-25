
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace intellishelf_mobile
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            // Basic validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorLabel.Text = "Username and password are required.";
                ErrorLabel.IsVisible = true;
                return;
            }

            var token = await FetchJwtToken(username, password);
            if (!string.IsNullOrEmpty(token))
            {
                // Store token and navigate to the main page
                Preferences.Set("JwtToken", token);
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                ErrorLabel.Text = "Invalid credentials.";
                ErrorLabel.IsVisible = true;
            }
        }

        private async Task<string> FetchJwtToken(string userName, string password)
        {
            try
            {
                var httpClient = new HttpClient();
                var loginData = new { userName, password };
                string json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://localhost:8080/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<JwtResponse>(responseContent);
                    return responseData?.token;
                }
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "An error occurred during login.";
                ErrorLabel.IsVisible = true;
            }

            return null;
        }

        public class JwtResponse
        {
            public string token { get; set; }
        }
    }
}
