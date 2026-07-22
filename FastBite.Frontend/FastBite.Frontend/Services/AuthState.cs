using FastBite.Models;

namespace FastBite.Services;

/// <summary>
/// Scoped service that holds the authenticated user and JWT token.
/// Inject this wherever you need to read/change auth state.
/// </summary>
public class AuthState
{
    public User? User { get; private set; }
    public string? Token { get; private set; }
    public bool IsAuthenticated => User is not null;

    public event Action? OnChange;

    public void SetUser(User user, string token)
    {
        User = user;
        Token = token;
        NotifyStateChanged();
    }

    public void Logout()
    {
        User = null;
        Token = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
