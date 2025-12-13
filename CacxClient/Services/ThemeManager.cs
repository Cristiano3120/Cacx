using CacxClient.Abstractions;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace CacxClient.Services;

internal sealed class ThemeManager(IPathProvider pathProvider)
{
    public const string LightTheme = "LightTheme.json";
    public const string DarkTheme = "DarkTheme.json";

    public static void SetToLightMode()
    {
        SetToSpecificMode(LightTheme);
    }

    public static void SetToDarkMode()
    {
        SetToSpecificMode(DarkTheme);
    }

    public static void SetToPinkMode()
    {
        throw new NotImplementedException("There is no pink preset yet");
    }

    private static void SetToSpecificMode(string filename)
    {
        string filepath = Path.Combine("Resources/Themes", filename);
        Theme? theme = JsonSerializer.Deserialize<Theme>(filepath);
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
        Color[] arr = [.. Application.Current.Resources.MergedDictionaries[0].Values.Cast<Color>()];
        Dictionary<string, Color> darkModeColors = [];
        int i = 0;

        foreach (string key in Application.Current.Resources.MergedDictionaries[0].Keys)
        {
            darkModeColors[key] = arr[i];
            i++;
        }

        Theme theme = new()
        {
            Name = "LightMode",
            Colors = darkModeColors
        };

        File.WriteAllText(path: pathProvider.GetPath("Resources/Themes/LightTheme.json"), JsonSerializer.Serialize(theme, new JsonSerializerOptions() { WriteIndented = true}));
    }

    private static void ApplyTheme(Theme theme)
    {
        foreach ((string? key, Color color) in theme.Colors)
        {
            Application.Current.Resources[key] = color;
        }
    }
}