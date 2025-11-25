using System.Windows.Data;
using System.Windows.Markup;

namespace CacxClient.ResourceManagment;

public class LocalizationExtension(string key) : MarkupExtension
{
    public string Key { get; init; } = key;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        Binding binding = new($"[{Key}]")
        {
            Source = new LocalizationBindingSource()
        };

        return binding.ProvideValue(serviceProvider);
    }
}
