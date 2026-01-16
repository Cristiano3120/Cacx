using Cacx.LocalizationManager.Abstractions;
using Cacx.LocalizationManager.Core;
using CacxClient.Resources;

namespace CacxClient.MVVM;

public sealed class VerificationViewModel
{
    public ILocalizationProvider Loc { get; }

    public VerificationViewModel()
    {
        Loc = new LocalizationProvider(resourceName: ResourceBasePaths.Verification, culture: null);
    }
}
