using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Intellishelf.Services.Implementation;

public static class UiNotifier
{
    public static void ShowError(string message)
    {
        var resources = Application.Current.Resources;

        Snackbar.Make(message, visualOptions: new SnackbarOptions
        {
            BackgroundColor       = (Color)resources["Magenta"],
            TextColor             = (Color)resources["White"],
            ActionButtonTextColor = (Color)resources["White"]
        }).Show();
    }

    public static void ShowSuccess(string message)
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