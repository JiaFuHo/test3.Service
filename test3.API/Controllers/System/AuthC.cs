using Microsoft.AspNetCore.Mvc;
using test3.API.Models.System;
using test3.Common;
using test3.Interface;

namespace test3.API.Controllers.System
{
    [ApiController]
    [Route("sys")]
    public class AuthC : ControllerBase
    {
        #region Fields
        private readonly AuthI _authP;
        private readonly ILogger<AuthC> _logO;
        #endregion

        #region Constructor
        public AuthC(AuthI authP, ILogger<AuthC> log)
        {
            _authP = authP;
            _logO = log;
        }
        #endregion

        #region Actions
        [HttpPost("auth")]
        public ActionResult<AuthRes> Auth([FromBody] AuthReq model)
        {
            var (status, token, message) = _authP.Auth(model.UAcc!, model.UPwd!);

            if (status)
            {
                _logX.L1();
                _logO.LogInformation($"{message}");

                return Ok(new AuthRes { Token = token });
            }
            else
            {
                _logX.L1();
                _logO.LogError($"{message}");

                return Unauthorized(new { Message = message });
            }
        }
        #endregion
    }
}