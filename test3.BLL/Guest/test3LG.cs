using test3.Common;
//using test3.DAL.HIS3.Context;
using test3.Dto;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace test3.BLL.Guest
{
    public class test3LG
    {
        #region Fields
        private readonly IConfiguration _config;
        private readonly ILogger<test3LG> _loggerO;
        private readonly IMemoryCache _cache;
        // db
        #endregion

        #region Constructor
        public test3LG(IConfiguration config, IMemoryCache cache, ILogger<test3LG> logger)
        {
            _config = config;
            _loggerO = logger;
            _cache = cache;
        }
        #endregion

        #region Methods

        #region Home

        #endregion

        #region Collection

        #endregion

        #region Search

        #endregion

        #endregion
    }
}