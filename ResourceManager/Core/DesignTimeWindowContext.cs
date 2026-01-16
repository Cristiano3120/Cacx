using LocalizationManager.Abstractions;

namespace LocalizationManager.Core;

public sealed class DesignTimeWindowContext
{
    public IDesignTimeLocalizationProvider Loc { get; } = new DesignTimeLocalizationProvider();
}
