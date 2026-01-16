using System.Globalization;
using System.Resources;

namespace LocalizationManager.Abstractions;

public interface ILocalizationProvider
{
    string GetString(string key);
    object GetObject(string key);
    Stream GetStream(string key);

    void UpdateContext(ResourceManager resourceManager);
    void UpdateContext(string resourceName);
    void UpdateCulture(CultureInfo culture);
    CultureInfo GetCulture();
}
