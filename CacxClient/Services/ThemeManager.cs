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

    public static void SetToPinkMode()
    {
        throw new NotImplementedException("There is no pink preset yet");
    }

    private void SetToSpecificMode(string filename)
    {
        string filepath = Path.Combine("Resources/Themes", filename);
        string content = File.ReadAllText(pathProvider.GetPath(filepath));

        Theme? theme = JsonSerializer.Deserialize<Theme>(content);
        if (theme is null || theme.Colors.Count == 0)
        {
            return;
        }

        ApplyTheme(theme);
    }


    // This method is only for creating the theme json file. Not used in production.
    //call after window creation in app.xaml.cs 
    //Change Path and Name
    public void CreateThemeTest()
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

        const string ThemeName = "DarkMode";
        Theme theme = new()
        {
            Name = ThemeName,
            Colors = colorsToSave
        };
        
        File.WriteAllText(path: pathProvider.GetPath($"Resources/Themes/{ThemeName}.json"), JsonSerializer.Serialize(theme, jsonSerializerOptions));
    }

    private static void ApplyTheme(Theme theme)
    {
        const string BrushCollectionName = "Brushes.xaml";
        ResourceDictionary brushDictonary = Application.Current.Resources.MergedDictionaries
            .First(x => x.Source.OriginalString.Contains(BrushCollectionName));

        foreach ((string key, Color color) in theme.Colors)
        {
            if (brushDictonary[key] is SolidColorBrush brush)
            {
                brush.Color = color;
            }
        }
    }
}