using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace test3.Common
{
    public static class AESHelper
    {
        #region Fields
        private static Byte[] _SK = Array.Empty<Byte>();
        private static Byte[] _IV = Array.Empty<Byte>();
        #endregion

        #region Methods
        public static void Init(String SK, String IV)
        {
            _SK = Encoding.UTF8.GetBytes(SK);
            _IV = Encoding.UTF8.GetBytes(IV);
        }

        public static String Decrypt(String cipherText)
        {
            if (String.IsNullOrEmpty(cipherText)) { return cipherText; }

            try
            {
                Byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes AES = Aes.Create())
                {
                    AES.Key = _SK;
                    AES.IV = _IV;
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;

                    var decryptor = AES.CreateDecryptor(AES.Key, AES.IV);

                    using (MemoryStream MS = new MemoryStream(buffer))
                    using (CryptoStream CS = new CryptoStream(MS, decryptor, CryptoStreamMode.Read))
                    using (StreamReader SR = new StreamReader(CS)) { return SR.ReadToEnd(); }
                }
            }
            catch { return cipherText; }
        }

        public static String Encrypt(String plainText)
        {
            if (String.IsNullOrEmpty(plainText)) { return plainText; }

            try
            {
                Byte[] buffer = Encoding.UTF8.GetBytes(plainText);

                using (Aes AES = Aes.Create())
                {
                    AES.Key = _SK;
                    AES.IV = _IV;
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;

                    var encryptor = AES.CreateEncryptor(AES.Key, AES.IV);

                    using (MemoryStream MS = new MemoryStream())
                    {
                        using (CryptoStream CS = new CryptoStream(MS, encryptor, CryptoStreamMode.Write))
                        {
                            CS.Write(buffer, 0, buffer.Length);
                            CS.FlushFinalBlock();
                        }

                        return Convert.ToBase64String(MS.ToArray());
                    }
                }
            }
            catch { return plainText; }
        }
        #endregion
    }

    public class Decryptor : JsonConverter<String>
    {
        #region Methods
        public override String? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var cipherText = reader.GetString();

            if (String.IsNullOrEmpty(cipherText)) { return cipherText; }

            return AESHelper.Decrypt(cipherText);
        }

        public override void Write(Utf8JsonWriter writer, String plainText, JsonSerializerOptions options)
        {
            writer.WriteStringValue(plainText);
        }
        #endregion
    }
}