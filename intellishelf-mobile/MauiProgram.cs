using System.Reflection;
using CommunityToolkit.Maui;
using Intellishelf.Clients;
using Intellishelf.Clients.Implementation;
using Intellishelf.Common;
using Intellishelf.Services;
using Intellishelf.Services.Implementation;
using Intellishelf.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Intellishelf;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .Single(n => n.EndsWith("appsettings.json"));
        using var stream = assembly.GetManifestResourceStream(resourceName)!;

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(config);

        builder.Services
            .Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

        builder.Services.AddSingleton<IAuthStorage, AuthStorage>();
        builder.Services.AddTransient<AuthHandler>();
        builder.Services.AddTransient<BooksViewModel>();

        builder.Services.AddHttpClient<IIntellishelfApiClient, IntellishelfApiClient>()
            .ConfigureHttpClient((sp, client) =>
            {
                var settings = sp
                    .GetRequiredService<IOptions<ApiSettings>>()
                    .Value;

                client.BaseAddress = new Uri(settings.BaseUrl);
            })
            .AddHttpMessageHandler<AuthHandler>();

        builder.Services.AddHttpClient<IIntellishelfAuthClient, IntellishelfAuthClient>()
            .ConfigureHttpClient((sp, client) =>
            {
                var settings = sp
                    .GetRequiredService<IOptions<ApiSettings>>()
                    .Value;

                client.BaseAddress = new Uri(settings.BaseUrl);
            });


#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        return app;
    }
}