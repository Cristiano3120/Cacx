using System.Text;
using System.Windows;
using System.Windows.Controls;

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

        border.BorderBrush = border.BorderBrush.Clone(); 
        textBox.GotFocus += (_, _) =>
        {
            textBox.Foreground = textBox.Foreground.PlayHoverAnimation();
            border.PlayHoverAnimation();
        };

        textBox.LostFocus += (_, _) =>
        {
            textBox.Foreground = textBox.Foreground.PlayUnhoverAnimation();
            border.PlayUnhoverAnimation();
        };
    }

    public static void DisableEmojiInput(this TextBox textBox)
    {
        textBox.PreviewTextInput += (_, args) =>
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

        DataObject.AddPastingHandler(textBox, static (_, args) =>
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
