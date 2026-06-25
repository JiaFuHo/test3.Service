using System.ComponentModel.DataAnnotations;

namespace test3.API.Models.System
{
    public class AuthReq
    {
        [Required]
        public string? UserAcc { get; set; }

        [Required]
        public string? UserPwd { get; set; }
    }

    public class AuthRes
    {
        public string? Token { get; set; }
    }
}