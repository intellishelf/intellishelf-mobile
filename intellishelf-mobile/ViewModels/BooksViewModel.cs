using System.Collections.ObjectModel;
using System.Windows.Input;
using Intellishelf.Models;
using Intellishelf.Models.Books;
using Intellishelf.Services;

namespace Intellishelf.ViewModels;

public class BooksViewModel : BindableObject
{
    private readonly IIntellishelfApiClient _client;
    private readonly IAuthStorage _tokenService;

    public ObservableCollection<Book> Books { get; } = new ObservableCollection<Book>();

    private int _currentPage = 0;
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
    
    private int _totalPages = 0;
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
    
    private int _pageSize = 20;
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
    
    private BookOrderBy _selectedOrder = BookOrderBy.Added;
    public BookOrderBy SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (_selectedOrder != value)
            {
                _selectedOrder = value;
                OnPropertyChanged();
                ResetAndRefresh();
            }
        }
    }
    
    private bool _isAscending = true;
    public bool IsAscending
    {
        get => _isAscending;
        set
        {
            if (_isAscending != value)
            {
                _isAscending = value;
                OnPropertyChanged();
                ResetAndRefresh();
            }
        }
    }
    
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnPropertyChanged();
                ((Command)LoadMoreCommand).ChangeCanExecute();
                ((Command)RefreshCommand).ChangeCanExecute();
            }
        }
    }
    
    public ICommand LoadMoreCommand { get; }
    public ICommand RefreshCommand { get; }
    
    public List<BookOrderBy> OrderOptions { get; } = Enum.GetValues(typeof(BookOrderBy)).Cast<BookOrderBy>().ToList();

    public BooksViewModel(IIntellishelfApiClient client, IAuthStorage tokenService)
    {
        _client = client;
        _tokenService = tokenService;

        LoadMoreCommand = new Command(
            async () => await LoadMoreBooks(),
            () => !IsBusy && CurrentPage < TotalPages);

        RefreshCommand = new Command(
            async () => await ResetAndRefresh(),
            () => !IsBusy);
    }

    public async Task LoadInitialData()
    {
        await ResetAndRefresh();
    }

    public async Task ResetAndRefresh()
    {
        Books.Clear();
        CurrentPage = 0;
        TotalPages = 0;
        await LoadMoreBooks();
    }

    public async Task LoadMoreBooks()
    {
        if (IsBusy || CurrentPage >= TotalPages && TotalPages != 0)
            return;

        IsBusy = true;

        try
        {
            var result = await _client.GetBooksPagedAsync(
                CurrentPage + 1,
                PageSize,
                SelectedOrder,
                IsAscending);

            if (result != null)
            {
                foreach (var book in result.Items)
                {
                    var bytes = await _client.GetImageContentAsync(_tokenService.GetUserId(), book.FileName);
                    book.CoverImageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    Books.Add(book);
                }

                CurrentPage = result.Page;
                TotalPages = result.TotalPages;
            }
        }
        catch (Exception ex)
        {
            // Handle error (could add a property for error messages)
            Console.WriteLine($"Error loading books: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

}