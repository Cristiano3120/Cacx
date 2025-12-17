using CacxClient.Abstractions;
using Cristiano3120.Logging;
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


    /// <summary>
    /// Reads all the Brushes from the Brush ResourceDictionary and saves them into a Theme json file.
    /// </summary>
    /// <param name="themeName"></param>
    public void CreateTheme(string themeName)
    {
        const string BrushCollectionName = "Brushes.xaml";
        ResourceDictionary brushDictonary = Application.Current.Resources.MergedDictionaries
            .First(x => x.Source.OriginalString.Contains(BrushCollectionName));

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
        const string BrushCollectionName = "Brushes.xaml";
        ResourceDictionary brushDictonary = Application.Current.Resources.MergedDictionaries
            .First(x => x.Source.OriginalString.Contains(BrushCollectionName));

        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach ((string key, Color color) in theme.Colors)
            {
                if (brushDictonary[key] is SolidColorBrush brush)
                {
                    brush.Color = color;
                }
            }
        });
    }
}