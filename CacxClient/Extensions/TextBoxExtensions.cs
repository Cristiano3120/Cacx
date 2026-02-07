using CacxClient.Abstractions;
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

    /// <summary>
    /// Attaches event handlers to the specified TextBox to prevent input of characters matching the given character
    /// type
    /// </summary>
    /// <remarks>This method blocks both direct text entry and pasted content that contains restricted
    /// characters, based on the specified character type. Ensure that the TextBox is initialized before calling this
    /// method.</remarks>
    /// <param name="textBox">The TextBox control to which character input restrictions will be applied. Cannot be null.</param>
    /// <param name="characterType">A value that specifies the type of characters to restrict from being entered into the TextBox.</param>
    public static void DisableCertainChars(this TextBox textBox, CharacterTypes characterType)
    {
        textBox.PreviewTextInput += (_, args) => args.Handled = CheckTextForIllegalInput(args.Text, characterType);

        DataObject.AddPastingHandler(textBox, (_, args) =>
        {
            if (!args.DataObject.GetDataPresent(DataFormats.UnicodeText)
                || args.DataObject.GetData(DataFormats.UnicodeText) is not string text)
            {
                return;
            }

            if (CheckTextForIllegalInput(text, characterType))
            {
                args.CancelCommand();
            }
        });
    }

    /// <summary>
    /// Determines whether the provided text contains any characters that are considered illegal based on the specified
    /// character type constraints.
    /// </summary>
    /// <remarks>This method evaluates the input text against the allowed character types, such as emojis,
    /// numbers, and letters. If illegal input is detected, it will return accordingly</remarks>
    /// <returns>true if the text contains illegal input according to the character type restrictions; otherwise, false.</returns>
    private static bool CheckTextForIllegalInput(string input, CharacterTypes illegalTypes)
    {
        if (illegalTypes.HasFlag(CharacterTypes.Emoji) && input.EnumerateRunes().Any(IsEmoji))
        {
            return true;
        }

        if (illegalTypes.HasFlag(CharacterTypes.Number) && input.Any(char.IsDigit))
        {
            return true;
        }

        if (illegalTypes.HasFlag(CharacterTypes.Text) && input.Any(char.IsLetter))
        {
            return true;
        }

        return false;
    }

    private static bool IsEmoji(Rune rune)
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