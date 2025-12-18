using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CacxClient.Extensions;

public static class TextBoxExtensions
{
    public static void EnableFocusAnimation(this TextBox textBox)
    {
        _ = textBox.ApplyTemplate();
        if (textBox.Template.FindName("Border", textBox) is not Border border)
        {
            return;
        }

        textBox.Foreground = textBox.Foreground.Clone();
        border.BorderBrush = border.BorderBrush.Clone();

        textBox.GotFocus += (_, __) =>
        {
            ColorAnimation fgAnim = new()
            {
                To = (Color)Application.Current.Resources["HoverColor"],
                Duration = TimeSpan.FromSeconds(0.3)
            };
            textBox.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);

            ColorAnimation borderAnim = new()
            {
                To = (Color)Application.Current.Resources["HoverColor"],
                Duration = TimeSpan.FromSeconds(0.5)
            };
            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);
        };

        textBox.LostFocus += (_, __) =>
        {
            ColorAnimation fgAnim = new()
            {
                To = (Color)Application.Current.Resources["TextPrimaryColor"],
                Duration = TimeSpan.FromSeconds(0.2)
            };
            textBox.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, fgAnim);

            ColorAnimation borderAnim = new()
            {
                To = (Color)Application.Current.Resources["BorderPrimaryColor"],
                Duration = TimeSpan.FromSeconds(0.2)
            };
            border.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnim);
        };
    }

    public static void DisableEmojiInput(this TextBox textBox)
    {
        textBox.PreviewTextInput += (sender, args) =>
        {
            foreach (char c in args.Text)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherSymbol)
                {
                    args.Handled = true;
                    break;
                }
            }
        };

        DataObject.AddPastingHandler(textBox, static (sender, args) =>
        {
            if (args.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                string text = (string)args.DataObject.GetData(DataFormats.UnicodeText);

                text.EnumerateRunes().Where(r => IsEmoji(r)).ToList().ForEach(_ => args.CancelCommand());
            }
        });

        static bool IsEmoji(Rune rune)
        {
            return rune.Value switch
            {
                >= 0x1F300 and <= 0x1FAFF => true,
                >= 0x2600 and <= 0x26FF => true,
                >= 0x2700 and <= 0x27BF => true,
                _ => false
            };
        }
    }
}
