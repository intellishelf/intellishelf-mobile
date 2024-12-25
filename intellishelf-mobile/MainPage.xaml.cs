using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace intellishelf_mobile;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);

        //
        // using var client = new HttpClient();
        // var result = await client.PostAsJsonAsync("https://intellistest-api.azurewebsites.net/api/auth/login", new
        // {
        //     userName = "maria",
        //     password = "guess"
        // });
        // CounterBtn.Text = "await result.Content.ReadAsStringAsync()";
    }
}