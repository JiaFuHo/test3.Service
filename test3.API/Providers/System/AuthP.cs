using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test3.Dto.Common;
using test3.Interface;

namespace test3.API.Providers.System
{
    public class AuthP : AuthI, LoginI
    {
        #region Fields
        private readonly IConfiguration _para;
        #endregion

        #region Constructor
        public AuthP(IConfiguration para) { _para = para; }
        #endregion

        #region Methods
        public (Boolean status, String? token, String message) Auth(String UAcc, String UPwd)
        {
            var JWT = _para.GetSection("JWT");

            var Acc = JWT["Acc"];
            var Pwd = JWT["Pwd"];
            var SK = JWT["SK"];

            if (UAcc != Acc || UPwd != Pwd) { return (false, null, "API驗證失敗"); }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, UAcc),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SK!));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDsc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = JWT["Issuer"],
                Audience = JWT["Audience"],
                SigningCredentials = sign
            };
            var tokenHdl = new JwtSecurityTokenHandler();
            var tokenStr = tokenHdl.WriteToken(tokenHdl.CreateToken(tokenDsc));

            return (true, tokenStr, "API驗證成功");
        }

        public String Login(LoginRes Res)
        {
            var JWT = _para.GetSection("JWT");

            var SK = JWT["SK"];

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(Res.Guid)!)
            };

            if (Res.Permission == 3) { claims.Add(new Claim(ClaimTypes.Role, "Client3")); }
            else if (Res.Permission == 2) { claims.Add(new Claim(ClaimTypes.Role, "Client2")); }
            else if (Res.Permission == 1) { claims.Add(new Claim(ClaimTypes.Role, "Client1")); }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SK!));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDsc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = JWT["Issuer"],
                Audience = JWT["Audience"],
                SigningCredentials = sign
            };
            var tokenHdl = new JwtSecurityTokenHandler();
            var tokenStr = tokenHdl.WriteToken(tokenHdl.CreateToken(tokenDsc));

            return tokenStr;
        }
        #endregion
    }
}
