using System.Security.Cryptography;

namespace CacxClient.RandomPasswordGenerator;

internal class PasswordGenerator
{
    private readonly Dictionary<CharacterCategory, string> _charMap = new()
    {
        { CharacterCategory.LowerCase, "abcdefghijklmnopqrstuvwxyz" },
        { CharacterCategory.UpperCase, "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
        { CharacterCategory.Digits, "0123456789" },
        { CharacterCategory.Brackets, "()" },
        { CharacterCategory.CurlyBrackets, "{}" },
        { CharacterCategory.Dollar, "$" },
        { CharacterCategory.ExclamationMark, "!" },
        { CharacterCategory.QuestionMark, "?" },
        { CharacterCategory.AtSign, "@" },
        { CharacterCategory.Hashtag, "#" },
        { CharacterCategory.Percent, "%" },
        { CharacterCategory.Dot, "." },
        { CharacterCategory.And, "&" },
        { CharacterCategory.Euro, "€" }
    };

    public string GeneratePassword(byte passwordLength)
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            List<char> allowedChars = [];
            foreach (KeyValuePair<CharacterCategory, string> kvp in _charMap)
            {
                if (kvp.Key is CharacterCategory.LowerCase or CharacterCategory.UpperCase)
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

            char[] password = new char[passwordLength];
            byte[] randomBytes = new byte[passwordLength];

            for (int i = 0; i < passwordLength; i++)
            {
                rng.GetBytes(randomBytes, 0, 1);
                int index = randomBytes[0] % allowedChars.Count;
                password[i] = allowedChars[index];
            }

            return new string(password);
        }
    }
}
