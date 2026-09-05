public static class AccountSession
{
    public static string Username { get; private set; } = "";
    public static string Password { get; private set; } = "";

    public static bool IsLoggedIn
    {
        get
        {
            return string.IsNullOrWhiteSpace(Username) == false
                && string.IsNullOrWhiteSpace(Password) == false;
        }
    }

    public static void Login(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public static void Logout()
    {
        Username = "";
        Password = "";
    }
}