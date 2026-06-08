using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test3.Api.Models;

namespace test3.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthC : ControllerBase
    {
        #region Fields
        private readonly IConfiguration _config;
        #endregion

        #region Constructor
        public AuthC(IConfiguration config) { _config = config; }
        #endregion

        #region Actions
        [HttpPost("login")]
        public ActionResult<AuthRes> Login([FromBody] AuthReq model)
        {
            if (model.UserAcc != "admin" || model.UserPwd != "test123") { return Unauthorized(new { Message = "登入失敗" }); }

            var jwt = _config.GetSection("JwtSettings");
            var SK = jwt["SK"];

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.UserAcc),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SK!));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDsc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwt["Issuer"],
                Audience = jwt["Audience"],
                SigningCredentials = sign
            };
            var tokenHdl = new JwtSecurityTokenHandler();
            var tokenStr = tokenHdl.WriteToken(tokenHdl.CreateToken(tokenDsc));

            return Ok(new AuthRes { Token = tokenStr });
        }
        #endregion
    }
}