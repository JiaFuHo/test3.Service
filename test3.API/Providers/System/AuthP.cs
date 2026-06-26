using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test3.API.Models.System;

namespace test3.API.Providers.System
{
    public interface IAuthP
    {
        (Boolean status, String? token, String message) LoginAuth(AuthReq model);
    }

    public class AuthP : IAuthP
    {
        #region Fields
        private readonly IConfiguration _config;
        #endregion

        #region Constructor
        public AuthP(IConfiguration config) { _config = config; }
        #endregion

        #region Methods
        public (Boolean status, String? token, String message) LoginAuth(AuthReq model)
        {
            var JWT = _config.GetSection("JWT");

            var Acc = JWT["Acc"];
            var Pwd = JWT["Pwd"];
            var SK = JWT["SK"];

            if (model.UserAcc != Acc || model.UserPwd != Pwd) { return (false, null, "API驗證失敗"); }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.UserAcc!),
                new Claim(ClaimTypes.Role, "Admin"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SK!));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDsc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = JWT["Issuer"],
                Audience = JWT["Audience"],
                SigningCredentials = sign,
            };
            var tokenHdl = new JwtSecurityTokenHandler();
            var tokenStr = tokenHdl.WriteToken(tokenHdl.CreateToken(tokenDsc));

            return (true, tokenStr, "API驗證成功");
        }
        #endregion
    }
}
