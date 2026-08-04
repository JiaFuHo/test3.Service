using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.BLL.Guest;
using test3.Common;
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
        [HttpGet("home/booklist")]
        public ActionResult<HomeQueryBookRes> GetBookList([FromQuery] HomeQueryBookReq model)
        {
            _loggerX.L1();

            var Res = new HomeQueryBookRes();

            var (validation, Req, statusCode, message) = HomeQueryBookValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _loggerO.LogError($"GetBookList驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = _logic.QueryBookList(Req!);

                if (Res.Status)
                {
                    var bookList = String.Join("、", Res.BookList!.Select(x => x.Title));

                    _loggerO.LogInformation($"GetBookList成功 - StatusCode = {Res.StatusCode}, BookList = {bookList}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5003";
                Res.Message = $"Service Error: {ex.Message}";

                _loggerO.LogError(ex, $"GetBookList錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            _loggerX.L1();

            return Ok(Res);
        }
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
            _loggerX.L1();

            var Res = new SearchQueryRes();

            var (validation, Req, statusCode, message) = SearchQueryValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _loggerO.LogError($"GetBookInfo驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = _logic.QueryBookInfo(Req!);

                if (Res.Status) { _loggerO.LogInformation($"GetBookInfo成功 - StatusCode = {Res.StatusCode}, Book = {Res.BookInfo!.Title}"); }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5003";
                Res.Message = $"Service Error: {ex.Message}";

                _loggerO.LogError(ex, $"GetBookInfo錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            _loggerX.L1();

            return Ok(Res);
        }
        #endregion

        #endregion

        #region Methods

        #region Home
        // Validation
        private (Boolean validation, HomeQueryBookReq? ReqModel, String? statusCode, String? message) HomeQueryBookValid(HomeQueryBookReq model)
        {
            if (String.IsNullOrWhiteSpace(model.Mode)) { return (false, null, "4001", "System Required Error"); }

            var modelX = model;

            return (true, modelX, null, null);
        }
        #endregion

        #region Collection

        #endregion

        #region Search
        // Validation
        private (Boolean validation, SearchQueryReq? ReqModel, String? statusCode, String? message) SearchQueryValid(SearchQueryReq model)
        {
            if (String.IsNullOrWhiteSpace(model.Info) && model.SYear == null && model.EYear == null && model.Lang == null && model.Type2 == null) { return (false, null, "4001", "Client Required Error: 任一查詢條件"); }

            var modelX = model;

            return (true, modelX, null, null);
        }
        #endregion

        #endregion
    }
}