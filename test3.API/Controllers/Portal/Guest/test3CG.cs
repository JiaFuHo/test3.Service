using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.BLL.Guest;
using test3.Dto.Guest;

namespace test3.API.Controllers.Portal
{
    [ApiController]
    [Route("guest")]
    [Authorize]
    public class test3CG : ControllerBase
    {
        #region Fields
        private readonly test3LG _logic;
        private readonly IConfiguration _config;
        private readonly ILogger<test3CG> _loggerO;
        #endregion

        #region Constructor
        public test3CG(test3LG logic, IConfiguration config, ILogger<test3CG> logger)
        {
            _logic = logic;
            _config = config;
            _loggerO = logger;
        }
        #endregion

        #region Actions

        #region Home

        #endregion

        #region Collection
        [HttpGet("collection")]
        public ActionResult<CollectionQueryRes> GetCollection()
        {
            var Res = new CollectionQueryRes();



            return Ok(Res);
        }
        #endregion

        #region Search
        [HttpGet("search")]
        public ActionResult<SearchQueryRes> GetBookInfo()
        {
            var Res = new SearchQueryRes();



            return Ok(Res);
        }
        #endregion

        #endregion

        #region Methods

        #endregion
    }
}