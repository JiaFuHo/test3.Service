using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using test3.DAL.test3.Context;
using test3.Dto.Admin;

namespace test3.BLL.Admin
{
    public class test3LA
    {
        #region Fields
        private readonly test3Context _db;
        private readonly ILogger<test3LA> _logO;
        #endregion

        #region Constructor
        public test3LA(test3Context db, ILogger<test3LA> log)
        {
            _db = db;
            _logO = log;
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