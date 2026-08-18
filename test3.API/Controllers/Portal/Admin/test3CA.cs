using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.BLL.Admin;
using test3.Common;
using test3.Dto.Admin;

namespace test3.API.Controllers.Portal.Admin
{
    [ApiController]
    [Route("admin")]
    [Authorize]
    public class test3CA : ControllerBase
    {
        #region Fields
        private readonly test3LA _logic;
        private readonly ILogger<test3CA> _logO;
        #endregion

        #region Constructor
        public test3CA(test3LA logic, ILogger<test3CA> log)
        {
            _logic = logic;
            _logO = log;
        }
        #endregion

        #region Actions

        #endregion

        #region Methods

        #endregion
    }
}