using CacxClient.Interfaces;
using CacxClient.ResourceManagment;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace CacxClient.Services.ResourceManagment;

public sealed class LocalizationService : INotifyPropertyChanged, ILocalizationService
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentUICulture;
    public static LocalizationService Instance => field ??= new LocalizationService();

    public void SetLanguage(LanguageCode languageCode)
    {
        string languageCodeStr = $"{languageCode}".ToLower();
        CurrentCulture = new CultureInfo(languageCodeStr);
        
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    public string GetString(string propertyName)
    {
        string[] parts = propertyName.Split('_'); //Example Login_Email
        string filename = $"Resources_de_{parts[0]}";
        string fullType = $"CacxClient.Resources.{CurrentCulture.TwoLetterISOLanguageName}.{filename}";

        Type? resType = Type.GetType(fullType);
        if (resType is null)
        {
            return $"MissingResx:{fullType}";
        }

        PropertyInfo? prop = resType.GetProperty(parts[1]);
        if (prop is null)
        {
            return $"MissingKey:{parts[1]}";
        }

        return prop.GetValue(null)?.ToString() ?? $"NullKey:{parts[1]}";
    }
}
