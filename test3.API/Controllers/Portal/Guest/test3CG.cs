using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test3.BLL.Common;
using test3.BLL.Guest;
using test3.Common;
using test3.Dto.Common;
using test3.Dto.Guest;

namespace test3.API.Controllers.Portal.Guest
{
    [ApiController]
    [Route("guest")]
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
        #endregion

        #region Collection
        [HttpGet("collection")]
        public async Task<ActionResult<CollectionQueryRes>> GetCollection([FromQuery] CollectionQueryReq model)
        {
            var Res = new CollectionQueryRes();

            try
            {
                Res = await _logicG.QueryCollection(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"GetCollection成功 - StatusCode = {Res.StatusCode}, BookCount = {Res.TotalCount}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetCollection錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }
        #endregion

        #region Info

        #region Fav
        [HttpGet("info/fav")]
        [Authorize]
        public async Task<ActionResult<FavQueryRes>> GetFav([FromQuery] FavQueryReq model)
        {
            var Res = new FavQueryRes();

            var (validation, Req, statusCode, message) = InfoQueryValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"GetFav驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.QueryFav(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"GetFav成功 - StatusCode = {Res.StatusCode}, FavCount = {Res.TotalCount}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetFav錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpPost("info/fav")]
        [Authorize]
        public async Task<ActionResult<FavSaveRes>> PostFav([FromBody] FavSaveReq model)
        {
            var Res = new FavSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"PostFav驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.CreateFav(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"PostFav成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"PostFav錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpDelete("info/fav")]
        [Authorize]
        public async Task<ActionResult<FavSaveRes>> DeleteFav([FromQuery] FavSaveReq model)
        {
            var Res = new FavSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"DeleteFav驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.DeleteFav(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"DeleteFav成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"DeleteFav錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }
        #endregion

        #region Rsv
        [HttpGet("info/rsv")]
        [Authorize]
        public async Task<ActionResult<RsvQueryRes>> GetRsv([FromQuery] RsvQueryReq model)
        {
            var Res = new RsvQueryRes();

            var (validation, Req, statusCode, message) = InfoQueryValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"GetRsv驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.QueryRsv(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"GetRsv成功 - StatusCode = {Res.StatusCode}, RsvCount = {Res.TotalCount}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetRsv錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpPost("info/rsv")]
        [Authorize]
        public async Task<ActionResult<RsvSaveRes>> PostRsv([FromBody] RsvSaveReq model)
        {
            var Res = new RsvSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"PostRsv驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.CreateRsv(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"PostRsv成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"PostRsv錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpPut("info/rsv")]
        [Authorize]
        public async Task<ActionResult<RsvSaveRes>> PutRsv([FromBody] RsvSaveReq model)
        {
            var Res = new RsvSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"PutRsv驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.UpdateRsv(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"PutRsv成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"PutRsv錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }
        #endregion

        #region Hx
        [HttpGet("info/hx")]
        [Authorize]
        public async Task<ActionResult<HxQueryRes>> GetHx([FromQuery] HxQueryReq model)
        {
            var Res = new HxQueryRes();

            var (validation, Req, statusCode, message) = InfoQueryValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"GetHx驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.QueryHx(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"GetHx成功 - StatusCode = {Res.StatusCode}, HxCount = {Res.TotalCount}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetHx錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpPost("info/hx")]
        [Authorize]
        public async Task<ActionResult<HxSaveRes>> PostHx([FromBody] HxSaveReq model)
        {
            var Res = new HxSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"PostHx驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.CreateHx(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"PostHx成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"PostHx錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpPut("info/hx")]
        [Authorize]
        public async Task<ActionResult<HxSaveRes>> PutHx([FromBody] HxSaveReq model)
        {
            var Res = new HxSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"PutHx驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.UpdateHx(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"PutHx成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"PutHx錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }
        #endregion

        #region Msg
        [HttpGet("info/msg")]
        [Authorize]
        public async Task<ActionResult<MsgQueryRes>> GetMsg([FromQuery] MsgQueryReq model)
        {
            var Res = new MsgQueryRes();

            var (validation, Req, statusCode, message) = InfoQueryValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"GetMsg驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.QueryMsg(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"GetMsg成功 - StatusCode = {Res.StatusCode}, MsgCount = {Res.TotalCount}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"GetMsg錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpPost("info/msg")]
        [Authorize]
        public async Task<ActionResult<MsgSaveRes>> PostMsg([FromBody] MsgSaveReq model)
        {
            var Res = new MsgSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"PostMsg驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.CreateMsg(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"PostMsg成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"PostMsg錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }

        [HttpPut("info/msg")]
        [Authorize]
        public async Task<ActionResult<MsgSaveRes>> PutMsg([FromBody] MsgSaveReq model)
        {
            var Res = new MsgSaveRes();

            var (validation, Req, statusCode, message) = InfoSaveValid(model);

            if (!validation)
            {
                Res.Status = false;
                Res.StatusCode = statusCode!;
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"PutMsg驗證失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Ok(Res);
            }

            try
            {
                Res = await _logicG.UpdateMsg(model);

                if (Res.Status)
                {
                    _logX.L1();
                    _logO.LogInformation($"PutMsg成功 - StatusCode = {Res.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"PutMsg錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }
        #endregion

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

        #region Info
        // Validation
        private (Boolean validation, InfoQueryReqBase? ReqModel, String? statusCode, String? message) InfoQueryValid(InfoQueryReqBase model)
        {
            if (model.Guid == null || model.Guid == Guid.Empty) { return (false, null, "4001", "System Required Error"); }

            var modelX = model;

            return (true, modelX, null, null);
        }

        private (Boolean validation, InfoSaveReqBase? ReqModel, String? statusCode, String? message) InfoSaveValid(InfoSaveReqBase model)
        {
            if (model.Guid == null || model.Guid == Guid.Empty) { return (false, null, "4001", "System Required Error"); }

            var modelX = (InfoSaveReqBase?)null;

            switch (model)
            {
                case FavSaveReq Fav:
                    if (Fav.CollectionId == null) { return (false, null, "4001", "System Required Error"); }

                    modelX = Fav; break;
                case RsvSaveReq Rsv:
                    if (Rsv.CollectionId == null) { return (false, null, "4001", "System Required Error"); }
                    if (Rsv.ReservationStatusId == null) { return (false, null, "4001", "System Required Error"); }

                    modelX = Rsv; break;
                case HxSaveReq Hx:
                    if (Hx.HistoryId == null) { return (false, null, "4001", "System Required Error"); }

                    modelX = Hx; break;
                case MsgSaveReq Msg:
                    if (Msg.NotificationId == null) { return (false, null, "4001", "System Required Error"); }
                    if (Msg.IsRead == null) { return (false, null, "4001", "System Required Error"); }

                    modelX = Msg; break;
                default:
                    return (false, null, "5101", "Service Error");
            }

            return (true, modelX, null, null);
        }
        #endregion

        #region Search
        // Validation
        private (Boolean validation, SearchQueryReq? ReqModel, String? statusCode, String? message) SearchQueryValid(SearchQueryReq model)
        {
            if (String.IsNullOrWhiteSpace(model.Info) && model.SYear == null && model.EYear == null && model.LangId == null && model.TypeId == null) { return (false, null, "4001", "Client Required Error: 任一查詢條件"); }

            var modelX = model;

            return (true, modelX, null, null);
        }
        #endregion

        #endregion
    }
}