using CacxClient.Abstractions;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CacxClient.Services;

internal static class ThemeManager
{
    public static void SetToLightMode()
    {

    }

    public static void SetToDarkMode()
    {

    }

    public static void SetToPinkMode()
    {

    }

    // This method is only for creating the theme json file. Not used in production.
    public static void CreateThemeTest()
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
            Name = "DarkMode",
            Colors = darkModeColors
        };

        File.WriteAllText("C:\\Users\\Crist\\source\\repos\\Cacx\\CacxClient\\Resources\\Themes\\DarkTheme.json", JsonSerializer.Serialize(theme, new JsonSerializerOptions() { WriteIndented = true}));
    }

    private static void ApplyTheme(Theme theme)
    {
        foreach ((string? key, Color color) in theme.Colors)
        {
            Application.Current.Resources[key] = color;
        }
    }
}