using System.ComponentModel.DataAnnotations;

namespace test3.API.Models.System
{
    public class AuthReq
    {
        [Required]
        public String? UAcc { get; set; }

        [Required]
        public String? UPwd { get; set; }
    }

    public class AuthRes
    {
        public String? Token { get; set; }
    }
}