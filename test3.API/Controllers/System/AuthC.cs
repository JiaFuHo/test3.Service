using Microsoft.AspNetCore.Mvc;
using test3.API.Models.System;
using test3.API.Providers.System;
using test3.BLL.Guest;
using test3.Common;

namespace test3.API.Controllers.System
{
    [ApiController]
    [Route("sys")]
    public class AuthC : ControllerBase
    {
        #region Fields
        private readonly IAuthP _auth;
        private readonly ILogger<AuthC> _loggerO;
        #endregion

        #region Constructor
        public AuthC(IAuthP authP, ILogger<AuthC> logger)
        {
            _auth = authP;
            _loggerO = logger;
        }
        #endregion

        #region Actions
        [HttpPost("auth")]
        public ActionResult<AuthRes> Login([FromBody] AuthReq model)
        {
            _loggerX.L1();
            _loggerO.LogInformation("API驗證開始");

            var (status, token, message) = _auth.LoginAuth(model);

            if (status)
            {
                _loggerX.L2();
                _loggerO.LogInformation($"{message}");

                return Ok(new AuthRes { Token = token });
            }
            else
            {
                _loggerX.L2();
                _loggerO.LogError($"{message}");

                return Unauthorized(new { Message = message });
            }
        }
        #endregion
    }
}