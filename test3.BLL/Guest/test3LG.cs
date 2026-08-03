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

            var (check, message) = SearchQueryChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4003";
                Res.Message = message!;

                _loggerO.LogError($"QueryBookInfo檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var querySrc = _db.Collections.AsQueryable();

            if (!String.IsNullOrWhiteSpace(Req.Info))
            {
                switch (Req.Type1)
                {
                    case "title":
                        querySrc = querySrc.Where(x => x.Title.Contains(Req.Info)); break;
                    case "author":
                        querySrc = querySrc.Where(x => x.Authors.Any(y => y.Author1.Contains(Req.Info))); break;
                    case "publisher":
                        querySrc = querySrc.Where(x => x.Publisher.Contains(Req.Info)); break;
                    case "isbn":
                        querySrc = querySrc.Where(x => x.Isbn == Req.Info); break;
                    default:
                        break;
                }

                if (!querySrc.Any())
                {
                    Res.Status = false;
                    Res.StatusCode = "4004";
                    Res.Message = Req.Type1 switch
                    {
                        "title" => "查無相關書名",
                        "author" => "查無相關作者",
                        "publisher" => "查無相關出版社",
                        "isbn" => "查無相關ISBN",
                        _ => "查無相關館藏"
                    };

                    _loggerO.LogError($"QueryBookInfo失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                    return Res;
                }
            }
            if (Req.SYear != null)
            {
                var SDate = new DateTime(Req.SYear.Value, 1, 1);

                querySrc = querySrc.Where(x => x.PublishDate >= SDate);
            }
            if (Req.EYear != null)
            {
                var EDate = new DateTime(Req.EYear.Value, 12, 31);

                querySrc = querySrc.Where(x => x.PublishDate <= EDate);
            }
            if (Req.Lang != null) { querySrc = querySrc.Where(x => x.LanguageId == Req.Lang); }
            if (Req.Type2 != null) { querySrc = querySrc.Where(x => x.TypeId == Req.Type2); }

            if (!querySrc.Any())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關館藏";

                _loggerO.LogError($"QueryBookInfo失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var query = querySrc.Select(x => new BookInfo
                {
                    Title = x.Title,
                    BDesc = x.Desc,
                    Image = x.Image,
                    Type = x.Type.Type1,
                    AuthorInfos = x.Authors.Select(y => new AuthorInfo { Author = y.Author1, ADesc = y.Desc }),
                    Translator = x.Translator,
                    Publisher = x.Publisher,
                    Language = x.Language.Language1,
                    ISBN = x.Isbn,
                    PublishDate = x.PublishDate
                });

                var bookInfo = query.FirstOrDefault();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = 1;
                Res.BookInfo = bookInfo;
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5002";
                Res.Message = $"System Error: {ex.Message}";

                _loggerO.LogError(ex, $"QueryBookInfo錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #endregion

        #region Aux Methods

        #region Home

        #endregion

        #region Collection

        #endregion

        #region Search
        // Check
        private (Boolean check, String? message) SearchQueryChk(SearchQueryReq model)
        {
            if (model.SYear > model.EYear) { return (false, "Logic Error: 年份 (起) > 年份 (迄)"); }

            return (true, null);
        }
        #endregion

        #endregion
    }
}