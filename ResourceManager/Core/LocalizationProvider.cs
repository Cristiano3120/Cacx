using LocalizationManager.Abstractions;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace LocalizationManager.Core;

public class LocalizationProvider : ILocalizationProvider, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private ResourceManager _resourceManager;
    private CultureInfo _culture;

    public string this[string key]
        => _resourceManager.GetString(key, _culture) ?? $"! MISSING KEY:{key}!";

    public LocalizationProvider(string resourceName, CultureInfo? culture)
    {
        _resourceManager = new ResourceManager(resourceName, Assembly.GetCallingAssembly());
        _culture = culture ?? CultureInfo.CurrentCulture;
    }

    public LocalizationProvider(ResourceManager resourceManager, CultureInfo? culture)
    {
        _resourceManager = resourceManager;
        _culture = culture ?? CultureInfo.CurrentCulture;
    }

    public void UpdateCulture(CultureInfo culture)
    {
        if (culture is null || culture.Equals(_culture))
            return;

        _culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
    public void UpdateContext(string resourceName)
    {
        _resourceManager.ReleaseAllResources();
        _resourceManager = new ResourceManager(baseName: resourceName, Assembly.GetCallingAssembly());
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    public void UpdateContext(ResourceManager resourceManager)
    {
        _resourceManager.ReleaseAllResources();
        _resourceManager = resourceManager;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    public string GetString(string key)
        => _resourceManager.GetString(key, _culture) ?? $"! MISSING KEY:{key}!";

    public object GetObject(string key)
        => _resourceManager.GetObject(key, _culture) ?? throw new MissingManifestResourceException($"Key '{key}' not found in resources.");

    public Stream GetStream(string key)
        => _resourceManager.GetStream(key, _culture) ?? throw new MissingManifestResourceException($"Key '{key}' not found in resources.");

    public CultureInfo GetCulture() => _culture;

    protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
