using Intellishelf.Clients;
using Intellishelf.Services;
using Microsoft.Extensions.Logging;

namespace Intellishelf;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register AuthHandler
        builder.Services.AddSingleton<AuthHandler>();

        // Register services
        builder.Services.AddSingleton<ITokenService, TokenService>();

        builder.Services.AddHttpClient<IIntellishelfApiClient, IntellishelfApiClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://intellishelf-test-fyhfe9bye5g2fud9.centralus-01.azurewebsites.net/api/");
            })
            .AddHttpMessageHandler<AuthHandler>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
