using Microsoft.AspNetCore.Mvc;
using test3.BLL.Common;
using test3.Common;
using test3.Dto.Common;

namespace test3.API.Controllers.Portal.Common
{
    [ApiController]
    public class LookupC : ControllerBase
    {
        #region Fields
        private readonly LookupL _logicL;
        private readonly ILogger<LookupC> _logO;
        #endregion

        #region Constructor
        public LookupC(LookupL logicL, ILogger<LookupC> log)
        {
            _logicL = logicL;
            _logO = log;
        }
        #endregion

        #region Actions
        [HttpGet("lookup")]
        public async Task<ActionResult<LookupRes>> Lookup()
        {
            var Res = new LookupRes();

            try
            {
                Res = await _logicL.Lookup();

                if (Res.Status)
                {
                    var typeList = String.Join("、", Res.TypeList!.Select(x => x.Type));
                    var publisherList = String.Join("、", Res.PublisherList!);
                    var langList = String.Join("、", Res.LangList!.Select(x => x.Lang));
                    var seriesList = String.Join("、", Res.SeriesList!.Select(x => x.Series));

                    _logX.L1();
                    _logO.LogInformation($"Lookup成功 - StatusCode = {Res.StatusCode}, TypeList = {typeList}, PublisherList = {publisherList}, LangList = {langList}, SeriesList = {seriesList}");
                }
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5101";
                Res.Message = $"Service Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"Lookup錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Ok(Res);
        }
        #endregion
    }
}