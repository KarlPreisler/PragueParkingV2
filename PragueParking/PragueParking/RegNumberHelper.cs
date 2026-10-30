namespace PragueParking;

public static class RegNumberHelper
{
    public const int MaxLength = 10;

    private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ÜÇĞŞİČĆĐŠŽØÆ";

    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "";
        }

        return text.Replace(" ", "").ToUpper();
    }

    public static bool IsValid(string regNumber)
    {
        if (string.IsNullOrEmpty(regNumber) || regNumber.Length > MaxLength)
        {
            return false;
        }

        foreach (char character in regNumber)
        {
            if (!AllowedCharacters.Contains(character))
            {
                return false;
            }
        }

        return true;
    }
}