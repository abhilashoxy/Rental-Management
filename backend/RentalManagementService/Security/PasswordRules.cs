// Security/PasswordRules.cs
using System.Text.RegularExpressions;

namespace RentalManagementService.Security
{
    public static class PasswordRules
    {
        // Example: min 8, upper, lower, digit, symbol
        public static bool IsStrong(string pwd) =>
            !string.IsNullOrWhiteSpace(pwd)
            && pwd.Length >= 8
            && Regex.IsMatch(pwd, "[A-Z]")
            && Regex.IsMatch(pwd, "[a-z]")
            && Regex.IsMatch(pwd, "[0-9]")
            && Regex.IsMatch(pwd, @"[\W_]");

        public static string Requirements =>
            "Minimum 8 characters with at least 1 uppercase, 1 lowercase, 1 digit, and 1 symbol.";
    }
}
