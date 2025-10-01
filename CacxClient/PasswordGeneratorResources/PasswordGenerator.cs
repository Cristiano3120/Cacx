using System.Security.Cryptography;

namespace CacxClient.PasswordGeneratorResources;

internal static class PasswordGenerator
{
    private static readonly Dictionary<Char, string> _charMap = new()
        {
            { Char.LowerCase, "abcdefghijklmnopqrstuvwxyz" },
            { Char.UpperCase, "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
            { Char.Digits, "0123456789" },
            { Char.Brackets, "()" },
            { Char.CurlyBrackets, "{}" },
            { Char.Dollar, "$" },
            { Char.ExclamationMark, "!" },
            { Char.QuestionMark, "?" },
            { Char.AtSign, "@" },
            { Char.Hashtag, "#" },
            { Char.Percent, "%" },
            { Char.Dot, "." },
            { Char.And, "&" },
            { Char.Euro, "€" }
        };

    public static string GeneratePassword()
    {
        //Fits perfectly into the textbox without scrolling
        const byte PasswordLength = 22;

        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            List<char> allowedChars = [];
            foreach (KeyValuePair<Char, string> kvp in _charMap)
            {
                if (kvp.Key is Char.LowerCase or Char.UpperCase)
                {
                    allowedChars.AddRange(kvp.Value);
                }
                else
                {
                    int repeatCharCount = Random.Shared.Next(2, 4);
                    for (int i = 0; i < repeatCharCount; i++)
                    {
                        allowedChars.AddRange(kvp.Value);
                    }
                }
            }

            char[] password = new char[PasswordLength];
            byte[] randomBytes = new byte[PasswordLength];

            for (int i = 0; i < PasswordLength; i++)
            {
                rng.GetBytes(randomBytes, 0, 1);
                int index = randomBytes[0] % allowedChars.Count;
                password[i] = allowedChars[index];
            }

            return new string(password);
        }
    }
}
