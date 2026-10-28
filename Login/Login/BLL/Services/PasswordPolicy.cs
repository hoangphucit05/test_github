using System;
using System.Linq;

namespace Login.BLL.Services
{
    public static class PasswordPolicy
    {
        public const string Requirements = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ và số.";

        public static bool IsValid(string? password)
        {
            return !string.IsNullOrWhiteSpace(password)
                && password.Length >= 8
                && password.Any(char.IsLetter)
                && password.Any(char.IsDigit);
        }
    }
}
