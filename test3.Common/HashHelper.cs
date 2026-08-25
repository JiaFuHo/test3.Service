using Isopoh.Cryptography.Argon2;

namespace test3.Common
{
    public static class HashHelper
    {
        #region Methods
        public static String? Hash(String plainText)
        {
            if (String.IsNullOrWhiteSpace(plainText)) { return null; }

            return Argon2.Hash(plainText);
        }

        public static Boolean Verify(String hash, String plainText)
        {
            if (String.IsNullOrWhiteSpace(hash) || String.IsNullOrWhiteSpace(plainText)) { return false; }

            return Argon2.Verify(hash, plainText);
        }
        #endregion
    }
}
