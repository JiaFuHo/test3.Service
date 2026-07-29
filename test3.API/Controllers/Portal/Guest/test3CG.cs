using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.BLL.Guest;
using test3.Dto.Guest;

namespace test3.API.Controllers.Portal
{
    [ApiController]
    [Route("guest")]
    //[Authorize]
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
        public ActionResult<SearchQueryRes> GetBookInfo([FromQuery] SearchQueryReq model)
        {
            var Res = new SearchQueryRes();

            var (validation, Req, statusCode, message) = SearchReqValid(model);

            if (!validation)
            {
                Res.Status = false; Res.StatusCode = statusCode!; Res.Message = message!;

                return Ok(Res);
            }

            try
            {
                Res = _logic.QueryBookInfo(Req!);

                if (Res.Status) { }
            }
            catch (Exception ex)
            {
                Res.Status = false; Res.StatusCode = "5003"; Res.Message = $"Service Error: {ex.Message}";
            }

            return Ok(Res);
        }
        #endregion

        #endregion

        #region Methods

        #region Home

        #endregion

        #region Collection

        #endregion

        #region Search
        // Validation
        private (Boolean validation, SearchQueryReq? ReqModel, String? statusCode, String? message) SearchReqValid(SearchQueryReq model)
        {
            var modelX = new SearchQueryReq
            {

            };

            return (true, modelX, null, null);
        }
        #endregion

        #endregion
    }
}