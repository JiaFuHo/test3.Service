using System.ComponentModel.DataAnnotations;

namespace test3.API.Models.System
{
    #region Encrypt
    public class EncryptReq
    {
        [Required]
        public String? PlainText { get; set; } = "";
    }

    public class EncryptRes
    {
        public String? CipherText { get; set; } = "";
    }
    #endregion
}