using Intellishelf.Clients;
using Intellishelf.Services;

namespace Intellishelf.Pages;

public partial class Settings
{
    public Settings(IIntellishelfAuthClient authClient, IAuthStorage tokenService)
    {
        InitializeComponent();
    }
}