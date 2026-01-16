namespace LocalizationManager.Abstractions;

public interface IDesignTimeLocalizationProvider
{
    string this[string key] { get; }
}
