using Intellishelf.Services.Implementation;
using Intellishelf.ViewModels;

namespace Intellishelf.Pages;

public partial class Books
{
    public Books(BooksViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.ErrorOccurred += OnErrorOccurred;
    }

    private void OnErrorOccurred(object? sender, string errorMessage)
    {
        UiNotifier.ShowError(errorMessage);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is BooksViewModel vm)
        {
            vm.ErrorOccurred -= OnErrorOccurred;
        }
    }
}