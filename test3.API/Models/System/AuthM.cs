using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace test3.API.Models.System
{
    public class AuthReq
    {
        [Required]
        [DefaultValue("admin")]
        public string? UserAcc { get; set; }

        [Required]
        [DefaultValue("test123")]
        public string? UserPwd { get; set; }
    }

    public class AuthRes
    {
        public string? Token { get; set; }
    }
}