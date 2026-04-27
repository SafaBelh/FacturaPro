namespace InvoicingApp.Services;

// 🟣🟣🟣 Singleton — one instance for the entire app lifetime 🟣🟣🟣 //

public class AuthService
{
    private bool _isLoggedIn = false;

    public bool Login(string email, string password)
    {
        if (email == "admin@facturapro.tn" && password == "admin123")
        {
            _isLoggedIn = true;
            return true;
        }
        return false;
    }

    public void Logout()
    {
        _isLoggedIn = false;
    }

    public bool IsLoggedIn => _isLoggedIn;
}