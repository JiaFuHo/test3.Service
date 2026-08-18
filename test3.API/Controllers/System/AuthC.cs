using Microsoft.AspNetCore.Mvc;
using test3.API.Models.System;
using test3.API.Providers.System;
using test3.Common;

namespace test3.API.Controllers.System
{
    [ApiController]
    [Route("sys")]
    public class AuthC : ControllerBase
    {
        #region Fields
        private readonly IAuthP _authP;
        private readonly ILogger<AuthC> _logO;
        #endregion

        #region Constructor
        public AuthC(IAuthP authP, ILogger<AuthC> log)
        {
            _authP = authP;
            _logO = log;
        }
        #endregion

        #region Actions
        [HttpPost("auth")]
        public ActionResult<AuthRes> Login([FromBody] AuthReq model)
        {
            _logX.L1();
            _logO.LogInformation("API驗證開始");

            var (status, token, message) = _authP.LoginAuth(model);

            if (status)
            {
                _logX.L2();
                _logO.LogInformation($"{message}");

                return Ok(new AuthRes { Token = token });
            }
            else
            {
                _logX.L2();
                _logO.LogError($"{message}");

                return Unauthorized(new { Message = message });
            }
        }
        #endregion
    }
}