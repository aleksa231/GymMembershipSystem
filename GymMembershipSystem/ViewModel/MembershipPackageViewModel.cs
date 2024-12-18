using System.ComponentModel;

public class MembershipPackageViewModel : INotifyPropertyChanged
{
    private string _packageName;
    private int? _durationMonths;
    private decimal? _price;      
    public string PackageName
    {
        get => _packageName;
        set
        {
            _packageName = value;
            OnPropertyChanged(nameof(PackageName));
        }
    }

    public int? DurationMonths
    {
        get => _durationMonths;
        set
        {
            _durationMonths = value;
            OnPropertyChanged(nameof(DurationMonths));
        }
    }

    public decimal? Price
    {
        get => _price;
        set
        {
            _price = value;
            OnPropertyChanged(nameof(Price));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
