
namespace Cyber_Vault.Utils;
internal class PasswordGenerator
{
    public static string Generate(int length, bool includeUpper, bool includeLower, bool includeNumbers, bool includeSpecialChars)
    {
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string numberChars = "0123456789";
        const string specialChars = "!@#$%^&*()_+";

        var allChars = "";

        if (includeUpper)
        {
            allChars += upperChars;
        }
        if (includeLower)
        {
            allChars += lowerChars;
        }
        if (includeNumbers)
        {
            allChars += numberChars;
        }
        if (includeSpecialChars)
        {
            allChars += specialChars;
        }

        if (string.IsNullOrEmpty(allChars))
        {
            return "";
        }

        var passwordChars = new char[length];
        var random = new Random();

        // Ensure each character set is included in the password
        if (includeUpper)
        {
            passwordChars[random.Next(length)] = upperChars[random.Next(upperChars.Length)];
        }
        if (includeLower)
        {
            passwordChars[random.Next(length)] = lowerChars[random.Next(lowerChars.Length)];
        }
        if (includeNumbers)
        {
            passwordChars[random.Next(length)] = numberChars[random.Next(numberChars.Length)];
        }
        if (includeSpecialChars)
        {
            passwordChars[random.Next(length)] = specialChars[random.Next(specialChars.Length)];
        }

        // Fill the rest of the password with random characters
        for (var i = 0; i < length; i++)
        {
            if (passwordChars[i] == '\0') // Check if the character is not already set
            {
                passwordChars[i] = allChars[random.Next(allChars.Length)];
            }
        }

        // Shuffle the password characters
        for (var i = length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (passwordChars[j], passwordChars[i]) = (passwordChars[i], passwordChars[j]);
        }

        return new string(passwordChars);
    }

}
