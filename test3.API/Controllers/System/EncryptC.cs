using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.API.Models.System;
using test3.Common;

namespace test3.API.Controllers.System
{
    [ApiController]
    [Route("sys")]
    [AllowAnonymous]
    public class EncryptC : ControllerBase
    {
        #region Actions

        #region Encrypt
        [HttpPost("encrypt")]
        public ActionResult<EncryptRes> Encrypt([FromBody] EncryptReq model)
        {
            var modelX = new EncryptRes();

            if (!String.IsNullOrWhiteSpace(model.PlainText)) { modelX.CipherText = AESHelper.Encrypt(model.PlainText); }

            return Ok(modelX);
        }
        #endregion

        #endregion
    }
}