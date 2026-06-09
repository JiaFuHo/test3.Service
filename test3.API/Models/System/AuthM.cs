namespace test3.API.Models.System
{
    public class AuthReq
    {
        public string UserAcc { get; set; } = "";
        public string UserPwd { get; set; } = "";
    }

    public class AuthRes
    {
        public required string Token { get; set; }
    }
}