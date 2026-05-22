using System;
using System.Security.Cryptography;
using System.Text;

namespace Financier.Desktop.Helpers
{
    /// <summary>
    /// Encrypts and decrypts sensitive settings values using DPAPI (CurrentUser scope).
    /// Cipher text is stored as Base64. Decryption falls back to returning the input as-is
    /// so that plain-text values written before this change are loaded without error.
    /// </summary>
    internal static class SettingsProtection
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Financier.Desktop.Settings.v1");

        internal static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cipherBytes);
        }

        internal static string TryDecrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);
                var plainBytes = ProtectedData.Unprotect(cipherBytes, Entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception)
            {
                // Not a DPAPI blob (e.g. migrating from a plain-text stored value): return as-is.
                return cipherText;
            }
        }
    }
}
