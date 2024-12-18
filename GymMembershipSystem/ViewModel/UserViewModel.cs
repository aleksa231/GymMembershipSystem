using System.ComponentModel;

public class UserViewModel : INotifyPropertyChanged
{
    private string _username;
    private string _passwordHash;
    private string _role;

    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(nameof(Username)); }
    }

    public string PasswordHash
    {
        get => _passwordHash;
        set { _passwordHash = value; OnPropertyChanged(nameof(PasswordHash)); }
    }

    public string Role
    {
        get => _role;
        set { _role = value; OnPropertyChanged(nameof(Role)); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
