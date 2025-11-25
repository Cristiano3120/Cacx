using CacxClient.ResourceManagment;

namespace CacxClient.Interfaces;

public interface ILocalizationService
{
    public void SetLanguage(LanguageCode languageCode);
}
