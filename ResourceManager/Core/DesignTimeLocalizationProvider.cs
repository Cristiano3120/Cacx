using LocalizationManager.Abstractions;

namespace LocalizationManager.Core;

public sealed class DesignTimeLocalizationProvider : IDesignTimeLocalizationProvider
{
    public string this[string key]
        => $"[{key}]";
}
