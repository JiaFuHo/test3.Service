using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using test3.DAL.test3.Context;
using test3.Dto.Admin;

namespace test3.BLL.Admin
{
    public class test3LA
    {
        #region Fields
        private readonly IConfiguration _config;
        private readonly ILogger<test3LA> _loggerO;
        private readonly IMemoryCache _cache;
        private readonly test3Context _db;
        #endregion

        #region Constructor
        public test3LA(IConfiguration config, ILogger<test3LA> logger, IMemoryCache cache, test3Context db)
        {
            _config = config;
            _loggerO = logger;
            _cache = cache;
            _db = db;
        }
        #endregion

        #region Methods

        #region

        #endregion

        #endregion

        #region Aux Methods

        #endregion
    }
}