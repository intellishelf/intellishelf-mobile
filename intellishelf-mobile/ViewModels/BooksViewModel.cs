using System.Collections.ObjectModel;
using System.Windows.Input;
using Intellishelf.Clients;
using Intellishelf.Models.Books;

namespace Intellishelf.ViewModels;

public class BooksViewModel : BindableObject
{
    private readonly IIntellishelfApiClient _client;

    public event EventHandler<string>? ErrorOccurred;

    public ObservableCollection<Book> Books { get; } = [];

    private int _currentPage;
    private int _totalPages;
    private int _pageSize = 2;
    private BookOrderBy _selectedOrder = BookOrderBy.Added;
    private bool _isAscending = true;
    private bool _isBusy;
    private bool _isRefreshing;

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalPages
    {
        get => _totalPages;
        set
        {
            if (_totalPages != value)
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (_pageSize != value)
            {
                _pageSize = value;
                OnPropertyChanged();
            }
        }
    }

    public BookOrderBy SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (_selectedOrder != value)
            {
                _selectedOrder = value;
                OnPropertyChanged();
                _ = Task.Run(async () => await ResetAndRefreshAsync());
            }
        }
    }

    public bool IsAscending
    {
        get => _isAscending;
        set
        {
            if (_isAscending != value)
            {
                _isAscending = value;
                OnPropertyChanged();
                _ = Task.Run(async () => await ResetAndRefreshAsync());
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnPropertyChanged();
                LoadMoreCommand.ChangeCanExecute();
                RefreshCommand.ChangeCanExecute();
            }
        }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            if (_isRefreshing != value)
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }
    }

    public Command LoadMoreCommand { get; }
    public Command RefreshCommand { get; }

    public List<BookOrderBy> OrderOptions { get; } =
        Enum.GetValues(typeof(BookOrderBy)).Cast<BookOrderBy>().ToList();

    public BooksViewModel(IIntellishelfApiClient client)
    {
        _client = client;

        LoadMoreCommand = new Command(
            () => _ = Task.Run(async () => await LoadMoreBooksAsync()),
            () => !IsBusy && (TotalPages == 0 || CurrentPage < TotalPages));

        RefreshCommand = new Command(
            () => _ = Task.Run(async () => await ResetAndRefreshAsync()),
            () => !IsRefreshing);

        _ = Task.Run(async () => await LoadMoreBooksAsync());
    }

    public async Task ResetAndRefreshAsync()
    {
        if (IsRefreshing)
            return;

        IsRefreshing = true;
        Books.Clear();
        CurrentPage = 0;
        TotalPages = 0;

        try
        {
            await LoadMoreBooksAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    public async Task LoadMoreBooksAsync()
    {
        if (IsBusy || (TotalPages > 0 && CurrentPage >= TotalPages))
            return;

        IsBusy = true;

        try
        {
            var result = await _client.GetBooksPagedAsync(
                CurrentPage + 1,
                PageSize,
                SelectedOrder,
                IsAscending);

            if (result.IsSuccess)
            {
                foreach (var book in result.Value.Items)
                    Books.Add(book);

                CurrentPage = result.Value.Page;
                TotalPages = result.Value.TotalPages;
            }
            else
            {
                ErrorOccurred?.Invoke(this, result.Error.Message);
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, $"Unexpected error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}