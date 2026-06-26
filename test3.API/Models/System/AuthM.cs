using System.ComponentModel.DataAnnotations;

namespace test3.API.Models.System
{
    public class AuthReq
    {
        [Required]
        public String? UserAcc { get; set; }

        [Required]
        public String? UserPwd { get; set; }
    }

    public class AuthRes
    {
        public String? Token { get; set; }
    }
}