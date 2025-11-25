using CacxClient.Services.ResourceManagment;
using System.Collections.Specialized;

namespace CacxClient.ResourceManagment;

public class LocalizationBindingSource : INotifyCollectionChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public string this[string key] => LocalizationService.Instance.GetString(key);

    public LocalizationBindingSource()
    {
        LocalizationService.Instance.PropertyChanged += (_, __) =>
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        };
    }
}
