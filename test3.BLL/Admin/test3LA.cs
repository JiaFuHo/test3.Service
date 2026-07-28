using test3.Common;
using test3.DAL.test3.Models;
using test3.Dto.Admin;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace test3.BLL.Admin
{
    public class test3LA
    {
        #region Fields
        private readonly IConfiguration _config;
        private readonly ILogger<test3LA> _loggerO;
        private readonly IMemoryCache _cache;
        // db
        #endregion

        #region Constructor
        public test3LA(IConfiguration config, IMemoryCache cache, ILogger<test3LA> logger)
        {
            _config = config;
            _loggerO = logger;
            _cache = cache;
        }
        #endregion

        #region Methods

        #region

        #endregion

        #endregion
    }
}