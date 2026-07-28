using test3.BLL.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace test3.API.Controllers.Portal.Admin
{
    [ApiController]
    [Route("admin")]
    [Authorize]
    public class test3CA : ControllerBase
    {
        #region Fields
        private readonly test3LA _logic;
        private readonly IConfiguration _config;
        private readonly ILogger<test3CA> _loggerO;
        #endregion

        #region Constructor
        public test3CA(test3LA logic, IConfiguration config, ILogger<test3CA> logger)
        {
            _logic = logic;
            _config = config;
            _loggerO = logger;
        }
        #endregion

        #region Actions

        #endregion

        #region Methods

        #endregion
    }
}