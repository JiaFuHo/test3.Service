using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.BLL.Common;
using test3.BLL.Guest;
using test3.Common;
using test3.Dto.Common;
using test3.Dto.Guest;

namespace test3.API.Controllers.Portal
{
    [ApiController]
    [Route("guest")]
    //[Authorize]
    public class test3CG : ControllerBase
    {
        #region Fields
        private readonly test3LG _logicG;
        private readonly LoginL _logicL;
        private readonly ILogger<test3CG> _logO;
        #endregion

        #region Constructor
        public test3CG(test3LG logicG, LoginL logicL, ILogger<test3CG> log)
        {
            _logicG = logicG;
            _logicL = logicL;
            _logO = log;
        }
        #endregion

        #region Actions

        #region Login
        [HttpPost("login")]
        public async Task<ActionResult<LoginRes>> Login([FromBody] LoginReq model)
        {
            var Res = new LoginRes();

            var (validation, Req, statusCode, message) = LoginValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"Login驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicL.Login(Req!);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"Login成功 - StatusCode = {Res.StatusCode}, Name = {Res.CName}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"Login錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }
        #endregion

        #region Home
        [HttpGet("home/booklist")]
        public async Task<ActionResult<HomeQueryBookRes>> GetBookList([FromQuery] HomeQueryBookReq model)
        {
            var Res = new HomeQueryBookRes();

            var (validation, Req, statusCode, message) = HomeQueryBookValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"GetBookList驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.QueryBookList(Req!);

                if (Res.Status)
                {
                    var bookList = String.Join("、", Res.BookList!.Select(x => x.Title));

                    _logX.L1();
                    _logO.LogInformation($"GetBookList成功 - StatusCode = {Res.StatusCode}, BookList = {bookList}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetBookList錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpGet("home/serieslist")]
        public async Task<ActionResult<HomeQuerySeriesRes>> GetSeriesList()
        {
            var Res = new HomeQuerySeriesRes();

            try
            {
                Res = await _logicG.QuerySeriesList();

                if (Res.Status)
                {
                    var seriesList = String.Join("、", Res.SeriesList!);

                    _logX.L1();
                    _logO.LogInformation($"GetSeriesList成功 - StatusCode = {Res.StatusCode}, SeriesList = {seriesList}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetSeriesList錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

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

        #region Info

        #endregion

        #region Search
        [HttpGet("search")]
        public async Task<ActionResult<SearchQueryRes>> GetBookInfo([FromQuery] SearchQueryReq model)
        {
            var Res = new SearchQueryRes();

            var (validation, Req, statusCode, message) = SearchQueryValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"GetBookInfo驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.QueryBookInfo(Req!);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"GetBookInfo成功 - StatusCode = {Res.StatusCode}, Book = {Res.BookInfo!.Title}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetBookInfo錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

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