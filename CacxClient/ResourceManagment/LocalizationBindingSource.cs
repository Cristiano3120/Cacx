using CacxClient.Services;
using System.ComponentModel;

namespace CacxClient.ResourceManagment;

public class LocalizationBindingSource : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string this[string key] => LocalizationService.Instance.GetString(key);

    public LocalizationBindingSource()
    {
        LocalizationService.Instance.PropertyChanged += (_, __) =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        };
    }
}
