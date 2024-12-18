using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Action<object> _executeWithParam;
    private readonly Action _executeWithoutParam;
    private readonly Predicate<object> _canExecute;

    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        _executeWithParam = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public RelayCommand(Action execute, Predicate<object> canExecute = null)
    {
        _executeWithoutParam = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        if (_executeWithParam != null)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        if (_executeWithParam != null)
        {
            _executeWithParam(parameter);
        }
        else
        {
            _executeWithoutParam();
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
