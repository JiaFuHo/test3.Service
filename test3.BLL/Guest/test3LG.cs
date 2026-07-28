using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using test3.DAL.test3.Context;
using test3.Dto.Guest;

namespace test3.BLL.Guest
{
    public class test3LG
    {
        #region Fields
        private readonly IConfiguration _config;
        private readonly ILogger<test3LG> _loggerO;
        private readonly IMemoryCache _cache;
        private readonly test3Context _db;
        #endregion

        #region Constructor
        public test3LG(IConfiguration config, ILogger<test3LG> logger, IMemoryCache cache, test3Context db)
        {
            _config = config;
            _loggerO = logger;
            _cache = cache;
            _db = db;
        }
        #endregion

        #region Methods

        #region Home

        #endregion

        #region Collection
        public CollectionQueryRes QueryCollection(CollectionQueryReq Req)
        {
            var Res = new CollectionQueryRes();



            return Res;
        }
        #endregion

        #region Search
        public SearchQueryRes QueryBookInfo(SearchQueryReq Req)
        {
            var Res = new SearchQueryRes();

            var querySrc = _db.Collections.AsQueryable();

            if (true) { }

            if (!querySrc.Any()) { Res.Status = false; Res.StatusCode = "4004"; Res.Message = ""; return Res; }

            return Res;
        }
        #endregion

        #endregion

        #region Aux Methods

        #endregion
    }
}