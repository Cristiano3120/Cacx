using System.Windows.Input;

namespace CacxClient.Commands;

public class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
{
    private readonly Action<object?> _execute = execute;
    private readonly Func<object?, bool>? _canExecute = canExecute;

    public bool CanExecute(object? parameter) 
        => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) 
        => _execute(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
