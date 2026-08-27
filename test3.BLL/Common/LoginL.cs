using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using test3.Common;
using test3.DAL.test3.Context;
using test3.Dto.Common;
using test3.Interface;

namespace test3.BLL.Common
{
    public class LoginL
    {
        #region Fields
        private readonly test3Context _db;
        private readonly LoginI _authP;
        private readonly ILogger<LoginL> _logO;
        #endregion

        #region Constructor
        public LoginL(test3Context db, LoginI authP, ILogger<LoginL> log)
        {
            _db = db;
            _authP = authP;
            _logO = log;
        }
        #endregion

        #region Methods
        public async Task<LoginRes> Login(LoginReq Req)
        {
            var Res = new LoginRes();

            try
            {
                var querySrc = await _db.Clients.SingleOrDefaultAsync(x => x.Cacc == Req.CAcc);

                if (querySrc == null)
                {
                    Res.Status = false;
                    Res.StatusCode = "4004";
                    Res.Message = "帳號 or 密碼錯誤";

                    _logX.L1();
                    _logO.LogError($"Login失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                    return Res;
                }

                var validation = HashHelper.Verify(querySrc.Cpwd, Req.CPwd!);

                if (!validation)
                {
                    Res.Status = false;
                    Res.StatusCode = "4005";
                    Res.Message = "帳號 or 密碼錯誤";

                    _logX.L1();
                    _logO.LogError($"Login失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                    return Res;
                }

                if (Req.Mode == "A" && querySrc.Permission == 1)
                {
                    Res.Status = false;
                    Res.StatusCode = "4006";
                    Res.Message = "權限不足";

                    _logX.L1();
                    _logO.LogError($"Login失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                    return Res;
                }

                Res.CId = querySrc.Cid;
                Res.Guid = querySrc.Guid;
                Res.CName = querySrc.Cname;
                Res.CPhone = querySrc.Cphone;
                Res.Permission = querySrc.Permission;
                Res.Token = _authP.Login(Res);

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "登入成功";
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"Login錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");

                return Res;
            }

            return Res;
        }
        #endregion
    }
}