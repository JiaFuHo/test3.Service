using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.BLL.Admin;
using test3.BLL.Common;
using test3.Common;
using test3.Dto.Admin;
using test3.Dto.Common;

namespace test3.API.Controllers.Portal.Admin
{
    [ApiController]
    [Route("admin")]
    [Authorize]
    public class test3CA : ControllerBase
    {
        #region Fields
        private readonly test3LA _logicA;
        private readonly LoginL _logicL;
        private readonly ILogger<test3CA> _logO;
        #endregion

        #region Constructor
        public test3CA(test3LA logicA, LoginL logicL, ILogger<test3CA> logO)
        {
            _logicA = logicA;
            _logicL = logicL;
            _logO = logO;
        }
        #endregion

        #region Actions

        #region Login
        [HttpGet("login")]
        public async Task<ActionResult<LoginRes>> Login([FromQuery] LoginReq model)
        {
            var Res = new LoginRes();

            var (validation, Req, statusCode, message) = LoginValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logO.LogError($"Login驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicL.Login(Req!);

                if (Res.Status) { _logO.LogInformation($"Login成功 - StatusCode = {Res.StatusCode}, Name = {Res.CName}"); }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logO.LogError(ex, $"Login錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            _logX.L1();

            return Ok(Res);
        }
        #endregion

        #endregion

        #region Methods

        #region Login
        // Validation
        private (Boolean validation, LoginReq? ReqModel, String? statusCode, String? message) LoginValid(LoginReq model)
        {
            if (String.IsNullOrWhiteSpace(model.Mode)) { return (false, null, "4001", "System Required Error"); }
            if (String.IsNullOrWhiteSpace(model.CAcc)) { return (false, null, "4001", "Client Required Error: 帳號"); }
            if (String.IsNullOrWhiteSpace(model.CPwd)) { return (false, null, "4001", "Client Required Error: 密碼"); }

            var modelX = model;

            return (true, modelX, null, null);
        }
        #endregion

        #endregion
    }
}