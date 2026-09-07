using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using test3.Common;
using test3.DAL.test3.Context;
using test3.Dto.Common;

namespace test3.BLL.Common
{
    public class LookupL
    {
        #region Fields
        private readonly test3Context _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<LookupL> _logO;
        #endregion

        #region Constructor
        public LookupL(test3Context db, IMemoryCache cache, ILogger<LookupL> log)
        {
            _db = db;
            _cache = cache;
            _logO = log;
        }
        #endregion

        #region Methods
        public async Task<LookupRes> Lookup()
        {
            var Res = new LookupRes();

            var CKey = $"Lookup";

            if (_cache.TryGetValue(CKey, out (List<TypeInfo>? CTypeList, List<String>? CPublisherList, List<LangInfo>? CLangList, List<SeriesInfo>? CSeriesList) CList))
            {
                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TypeList = CList.CTypeList;
                Res.PublisherList = CList.CPublisherList;
                Res.LangList = CList.CLangList;
                Res.SeriesList = CList.CSeriesList;

                return Res;
            }

            var querySrc1 = _db.Types.AsQueryable();
            var querySrc2 = _db.Collections.GroupBy(x => x.Publisher).OrderByDescending(g => g.Count());
            var querySrc3 = _db.Languages.AsQueryable();
            var querySrc4 = _db.Series.AsQueryable().OrderBy(x => x.SeriesId);

            try
            {
                var query1 = querySrc1.Select(x => new TypeInfo { TypeId = x.TypeId, Type = x.Type1 });
                var query2 = querySrc2.Select(g => g.Key).Take(5);
                var query3 = querySrc3.Select(x => new LangInfo { LangId = x.LanguageId, Lang = x.Language1 });
                var query4 = querySrc4.Select(x => new SeriesInfo { SeriesId = x.SeriesId, Series = x.Series1 }).Take(5);

                var typeList = await query1.ToListAsync();
                var publisherList = await query2.ToListAsync();
                var languageList = await query3.ToListAsync();
                var seriesList = await query4.ToListAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TypeList = typeList;
                Res.PublisherList = publisherList;
                Res.LangList = languageList;
                Res.SeriesList = seriesList;

                var COpt = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CKey, (typeList, publisherList, languageList, seriesList), COpt);
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"Lookup錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion
    }
}