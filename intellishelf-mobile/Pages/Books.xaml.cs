using System.Collections.ObjectModel;
using System.ComponentModel;
using Intellishelf.Clients;
using Intellishelf.Infra;
using Intellishelf.Models;
using Intellishelf.Models.Books;

namespace Intellishelf.Pages;

public partial class Books : ContentPage, INotifyPropertyChanged
{
    private readonly IIntellishelfApiClient _client;

    // Collections and state
    public ObservableCollection<Book> BooksList { get; } = [];
    
    private int _currentPage = 0;
    private int _totalPages = 0;
    private const int PageSize = 20;
    private BookOrderBy _selectedOrder = BookOrderBy.Added;
    private bool _isAscending = true;
    private bool _isBusy;

    // Order options for picker
    private readonly List<BookOrderBy> _orderOptions = Enum.GetValues(typeof(BookOrderBy)).Cast<BookOrderBy>().ToList();

    public Books(IIntellishelfApiClient client)
    {
        InitializeComponent();
        _client = client;
        
        InitializeControls();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BooksList.Count == 0)
        {
            await LoadBooksAsync();
        }
    }

    private void InitializeControls()
    {
        // Set up picker
        OrderByPicker.ItemsSource = _orderOptions;
        OrderByPicker.SelectedItem = _selectedOrder;
        
        // Set up checkbox
        AscendingCheckBox.IsChecked = _isAscending;
        
        // Set up collection view
        BooksCollection.ItemsSource = BooksList;
        
    }


    private void UpdateLoadingState(bool isBusy)
    {
        _isBusy = isBusy;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LoadingIndicator.IsRunning = isBusy;
            LoadingIndicator.IsVisible = isBusy;
            BooksRefreshView.IsRefreshing = false;
        });
    }

    // Event handlers
    private async void OnOrderByChanged(object sender, EventArgs e)
    {
        if (OrderByPicker.SelectedItem is BookOrderBy newOrder && newOrder != _selectedOrder)
        {
            _selectedOrder = newOrder;
            await RefreshBooksAsync();
        }
    }

    private async void OnAscendingChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value != _isAscending)
        {
            _isAscending = e.Value;
            await RefreshBooksAsync();
        }
    }

    private async void OnRefreshRequested(object sender, EventArgs e)
    {
        await RefreshBooksAsync();
    }

    private async void OnLoadMoreRequested(object sender, EventArgs e)
    {
        await LoadMoreBooksAsync();
    }


    // Core functionality
    private async Task RefreshBooksAsync()
    {
        if (_isBusy) return;

        BooksList.Clear();
        _currentPage = 0;
        _totalPages = 0;
        
        await LoadBooksAsync();
    }

    private async Task LoadMoreBooksAsync()
    {
        if (_isBusy || (_totalPages > 0 && _currentPage >= _totalPages))
            return;

        await LoadBooksAsync();
    }

    private async Task LoadBooksAsync()
    {
        if (_isBusy) return;

        PagedResult<Book> result = null;

        UpdateLoadingState(true);

        try
        {
            result = await _client.GetBooksPagedAsync(
                _currentPage + 1,
                PageSize,
                _selectedOrder,
                _isAscending);
        }
        catch (UserSessionExpiredException e)
        {
            Console.WriteLine(e);

            await Shell.Current.GoToAsync("//Login");
        }

        if (result != null)
        {
            foreach (var book in result.Items)
            {
                BooksList.Add(book);
            }

            _currentPage = result.Page;
            _totalPages = result.TotalPages;
            }

        UpdateLoadingState(false);
    }

    // INotifyPropertyChanged implementation (minimal for this POC)
    public new event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}