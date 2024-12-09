using System.Collections;
using System.ComponentModel;

namespace Lab1.ViewModel;

public class ErrorsViewModel : INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new ();

    public bool HasErrors => _errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        return _errors.GetValueOrDefault(propertyName, null);
    }

    public void AddError(string propertyName, string errorMessage)
    {
        if (!_errors.ContainsKey(propertyName))
        {
            _errors.Add(propertyName, new List<string>());
        }

        _errors[propertyName].Add(errorMessage);
        OnErrorsChanged(propertyName);
    }

    public void ClearErrors(string propertyName)
    {
        if (_errors.Remove(propertyName))
        {
            OnErrorsChanged(propertyName);
        }
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
