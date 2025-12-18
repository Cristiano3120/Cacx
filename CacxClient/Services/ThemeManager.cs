using CacxClient.Abstractions;
using System.Collections;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace CacxClient.Services;

internal sealed class ThemeManager(IPathProvider pathProvider, JsonSerializerOptions jsonSerializerOptions)
{
    public const string LightTheme = "LightMode.json";
    public const string DarkTheme = "DarkMode.json";

    public void SetToLightMode()
    {
        SetToSpecificMode(LightTheme);
    }

    public void SetToDarkMode()
    {
        SetToSpecificMode(DarkTheme);
    }

    private void SetToSpecificMode(string filename)
    {
        string filepath = Path.Combine("Resources/Themes", filename);
        string content = File.ReadAllText(pathProvider.GetPath(filepath));

        Theme? theme = JsonSerializer.Deserialize<Theme>(content, jsonSerializerOptions);
        if (theme is null || theme.Colors.Count == 0)
        {
            return;
        }

        ApplyTheme(theme);
    }

    private static ResourceDictionary GetBrushesDictionary()
    {
        const string BrushCollectionName = "Brushes.xaml";
        return GetResourceDictionary(BrushCollectionName);
    }

    private static ResourceDictionary GetColorsDictionary()
    {
        const string ColorCollectionName = "Colors.xaml";
        return GetResourceDictionary(ColorCollectionName);
    }

    private static ResourceDictionary GetResourceDictionary(string collectionName)
        => Application.Current.Resources.MergedDictionaries
            .First(x => x.Source.OriginalString.Contains(collectionName));


    /// <summary>
    /// Retrieves the color associated with the specified key from the application's color resource dictionary.
    /// </summary>
    /// <param name="key">The key that identifies the color resource to retrieve. Cannot be null.</param>
    /// <returns>A <see cref="Color"/> value if the key exists and is associated with a color; otherwise, <see langword="null"/>.</returns>
    public static Color? GetColor(string key)
    {
        ResourceDictionary colorDictonary = GetColorsDictionary();
        if (colorDictonary[key] is Color color)
        {
            return color;
        }

        return null;
    }

    /// <summary>
    /// Reads all the Brushes from the Brush ResourceDictionary and saves them into a Theme json file.
    /// </summary>
    /// <param name="themeName"></param>
    public void CreateTheme(string themeName)
    {
        ResourceDictionary brushDictonary = GetBrushesDictionary();

        Dictionary<string, Color> colorsToSave = [];
        foreach (DictionaryEntry dictionaryEntry in brushDictonary)
        {
            if (dictionaryEntry.Key is string key
                && dictionaryEntry.Value is SolidColorBrush brush)
            {
                colorsToSave[key] = brush.Color;
            }
        }

        Theme theme = new()
        {
            Name = themeName,
            Colors = colorsToSave
        };

        string path = pathProvider.GetPath($"Resources/Themes/{themeName}.json");
        File.WriteAllText(path, contents: JsonSerializer.Serialize(theme, jsonSerializerOptions));
    }

    private static void ApplyTheme(Theme theme)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ResourceDictionary brushDictonary = GetBrushesDictionary();
            ResourceDictionary colorDictonary = GetColorsDictionary();

            foreach ((string key, Color color) in theme.Colors)
            {
                if (brushDictonary[key] is SolidColorBrush brush)
                {
                    brush.Color = color;
                }

                string colorKey = key.Replace("Brush", "Color"); //Brushes always end in "Brush", Colors in "Color"
                colorDictonary[colorKey] = color;
            }
        });
    }
}