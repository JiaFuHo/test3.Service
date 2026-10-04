using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using test3.Common;
using test3.DAL.test3.Context;
using test3.DAL.test3.Models;
using test3.Dto.Guest;

namespace test3.BLL.Guest
{
    public class test3LG
    {
        #region Fields
        private readonly test3Context _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<test3LG> _logO;
        #endregion

        #region Constructor
        public test3LG(test3Context db, IMemoryCache cache, ILogger<test3LG> log)
        {
            _db = db;
            _cache = cache;
            _logO = log;
        }
        #endregion

        #region Methods

        #region Home
        public async Task<HomeQueryBookRes> QueryBookList(HomeQueryBookReq Req)
        {
            var Res = new HomeQueryBookRes();

            var CKey = $"BookList{Req.Mode}";

            if (_cache.TryGetValue(CKey, out List<BookInfo>? CBookList))
            {
                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = CBookList!.Count;
                Res.BookList = CBookList;

                return Res;
            }

            var querySrc = _db.Collections.AsQueryable();

            if (Req.Mode == "N") { querySrc = querySrc.OrderByDescending(x => x.Books.Max(y => y.AccessDate)).Take(9); }
            else { querySrc = querySrc.OrderByDescending(x => x.Books.Sum(y => y.Borrows.Count())).Take(9); }

            if (!await querySrc.AnyAsync())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關資訊";

                _logX.L1();
                _logO.LogError($"QueryBookList失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var query = querySrc.Select(x => new BookInfo
                {
                    CollectionId = x.CollectionId,
                    Title = x.Title,
                    BDesc = x.Desc,
                    Image = x.Image,
                    Type = x.Type.Type1,
                    AuthorInfos = x.Authors.Select(y => new AuthorInfo { Author = y.Author1 }),
                    Publisher = x.Publisher,
                    Language = x.Language.Language1,
                    ISBN = x.Isbn,
                    BookStatus = x.Books.Any(y => (y.BookStatusId == 1))
                });

                var bookList = await query.ToListAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = bookList.Count;
                Res.BookList = bookList;

                var COpt = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CKey, bookList, COpt);
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"QueryBookList錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #region Collection
        public async Task<CollectionQueryRes> QueryCollection(CollectionQueryReq Req)
        {
            var Res = new CollectionQueryRes();

            var (check, message) = CollectionQueryChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4003";
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"QueryCollection檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var querySrc = _db.Collections.AsQueryable();

            if (Req.TypeId != null) { querySrc = querySrc.Where(x => x.TypeId == Req.TypeId); }
            if (!String.IsNullOrWhiteSpace(Req.Publisher)) { querySrc = querySrc.Where(x => x.Publisher == Req.Publisher); }
            if (Req.LangId != null) { querySrc = querySrc.Where(x => x.LanguageId == Req.LangId); }
            if (Req.SeriesId != null) { querySrc = querySrc.Where(x => x.SeriesId == Req.SeriesId); }
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

            if (!await querySrc.AnyAsync())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關館藏";

                _logX.L1();
                _logO.LogError($"QueryCollection失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var querySrcX = querySrc.Skip((Req.Page - 1) * Req.Size).Take(Req.Size);

                var query = querySrcX.Select(x => new BookInfo
                {
                    CollectionId = x.CollectionId,
                    Title = x.Title,
                    Image = x.Image,
                    Type = x.Type.Type1,
                    AuthorInfos = x.Authors.Select(y => new AuthorInfo { Author = y.Author1 }),
                    Translator = x.Translator,
                    Publisher = x.Publisher,
                    Language = x.Language.Language1,
                    ISBN = x.Isbn,
                    BookStatus = x.Books.Any(y => (y.BookStatusId == 1) && (!y.Reservations.Any(z => z.ReservationStatusId == 1 || z.ReservationStatusId == 3)))
                });

                var bookList = await query.ToListAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = await querySrc.CountAsync();
                Res.BookList = bookList;
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"QueryCollection錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #region Info

        #region Fav
        public async Task<FavQueryRes> QueryFav(FavQueryReq Req)
        {
            var Res = new FavQueryRes();

            var querySrc = _db.Clients.Where(x => x.Guid == Req.Guid).SelectMany(x => x.Favorites);

            if (!await querySrc.AnyAsync())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"QueryFav失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var querySrcX = querySrc.Skip((Req.Page - 1) * Req.Size).Take(Req.Size);

                var query = querySrcX.Select(x => new FavInfo
                {
                    CollectionId = x.CollectionId,
                    Title = x.Collection.Title,
                    Image = x.Collection.Image,
                    AuthorInfos = x.Collection.Authors.Select(y => new AuthorInfo { Author = y.Author1 }),
                    Publisher = x.Collection.Publisher
                });

                var favList = await query.ToListAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = await querySrc.CountAsync();
                Res.FavList = favList;
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"QueryFav錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }

        public async Task<FavSaveRes> CreateFav(FavSaveReq Req)
        {
            var Res = new FavSaveRes();

            var (check, Cid, message) = await InfoSaveChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = message;

                _logX.L1();
                _logO.LogError($"CreateFav檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var createChk = await _db.Favorites.AnyAsync(x => x.Cid == (Int32)Cid! && x.CollectionId == Req.CollectionId);

            if (createChk)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "新增失敗，查有重複紀錄";

                _logX.L1();
                _logO.LogError($"CreateFav失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var create = new Favorite
                {
                    Cid = (Int32)Cid!,
                    CollectionId = (Int32)Req.CollectionId!
                };

                _db.Favorites.Add(create);
                await _db.SaveChangesAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "新增成功";
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"CreateFav錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }

        public async Task<FavSaveRes> DeleteFav(FavSaveReq Req)
        {
            var Res = new FavSaveRes();

            var (check, Cid, message) = await InfoSaveChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = message;

                _logX.L1();
                _logO.LogError($"DeleteFav檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var deleteSrc = await _db.Favorites.FirstOrDefaultAsync(x => x.Cid == (Int32)Cid! && x.CollectionId == Req.CollectionId);

            if (deleteSrc == null)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"DeleteFav失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                _db.Favorites.Remove(deleteSrc);
                await _db.SaveChangesAsync();

                Res.Status = true;
                Res.Message = "刪除成功";
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"DeleteFav錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #region Rsv
        public async Task<RsvQueryRes> QueryRsv(RsvQueryReq Req)
        {
            var Res = new RsvQueryRes();

            var querySrc = _db.Clients.Where(x => x.Guid == Req.Guid).SelectMany(x => x.Reservations);

            if (!await querySrc.AnyAsync())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"QueryRsv失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var querySrcX = querySrc.Skip((Req.Page - 1) * Req.Size).Take(Req.Size);

                var query = querySrcX.Select(x => new RsvInfo
                {
                    CollectionId = x.CollectionId,
                    Title = x.Collection.Title,
                    Image = x.Collection.Image,
                    AuthorInfos = x.Collection.Authors.Select(y => new AuthorInfo { Author = y.Author1 }),
                    ReservateDate = x.ReservateDate,
                    DueDateR = x.DueDateR,
                    ReservationStatus = x.ReservationStatus.ReservationStatus1
                });

                var rsvList = await query.ToListAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = await querySrc.CountAsync();
                Res.RsvList = rsvList;
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"QueryRsv錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }

        public async Task<RsvSaveRes> CreateRsv(RsvSaveReq Req)
        {
            var Res = new RsvSaveRes();

            var (check1, Cid, message1) = await InfoSaveChk(Req);

            if (!check1)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = message1;

                _logX.L1();
                _logO.LogError($"CreateRsv失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var (check2, message2) = await RsvChk((Int32)Cid!);

            if (!check2)
            {
                Res.Status = false;
                Res.StatusCode = "4003";
                Res.Message = message2;

                _logX.L1();
                _logO.LogError($"CreateRsv檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var createChk = await _db.Reservations.AnyAsync(x => x.Cid == (Int32)Cid! && x.CollectionId == Req.CollectionId && (x.ReservationStatusId == 1 || x.ReservationStatusId == 3));

            if (createChk)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "預約失敗，查有重複紀錄";

                _logX.L1();
                _logO.LogError($"CreateRsv失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var create = new Reservation
                {
                    Cid = (Int32)Cid!,
                    CollectionId = (Int32)Req.CollectionId!,
                    ReservateDate = DateTime.Today,
                    ReservationStatusId = (Byte)Req.ReservationStatusId!
                };

                _db.Reservations.Add(create);
                await _db.SaveChangesAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "預約成功";
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"CreateRsv錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }

        public async Task<RsvSaveRes> UpdateRsv(RsvSaveReq Req)
        {
            var Res = new RsvSaveRes();

            var (check, Cid, message) = await InfoSaveChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = message;

                _logX.L1();
                _logO.LogError($"UpdateRsv檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var updateSrc = await _db.Reservations.FirstOrDefaultAsync(x => x.Cid == (Int32)Cid! && x.CollectionId == Req.CollectionId && (x.ReservationStatusId == 1 || x.ReservationStatusId == 3));

            if (updateSrc == null)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"UpdateRsv失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                updateSrc.ReservationStatusId = (Byte)Req.ReservationStatusId!;
                await _db.SaveChangesAsync();

                Res.Status = true;
                Res.Message = "修改成功";
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"UpdateRsv錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #region Hx
        public async Task<HxQueryRes> QueryHx(HxQueryReq Req)
        {
            var Res = new HxQueryRes();

            var querySrc = _db.Clients.Where(x => x.Guid == Req.Guid).SelectMany(x => x.Borrows);

            if (!await querySrc.AnyAsync())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"QueryHx失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var querySrcX = querySrc.Skip((Req.Page - 1) * Req.Size).Take(Req.Size);

                var query = querySrcX.Select(x => new HxInfo
                {
                    HistoryId = x.History!.HistoryId,
                    Title = x.Book.Collection.Title,
                    Image = x.Book.Collection.Image,
                    AuthorInfos = x.Book.Collection.Authors.Select(y => new AuthorInfo { Author = y.Author1 }),
                    BorrowDate = x.BorrowDate,
                    DueDateB = x.DueDateB,
                    BorrowStatus = x.BorrowStatus.BorrowStatus1,
                    Score = x.History!.Score,
                    Feedback = x.History!.Feedback
                });

                var hxList = await query.ToListAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = await querySrc.CountAsync();
                Res.HxList = hxList;
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"QueryHx錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }

        public async Task<HxSaveRes> UpdateHx(HxSaveReq Req)
        {
            var Res = new HxSaveRes();

            var (check, Cid, message) = await InfoSaveChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = message;

                _logX.L1();
                _logO.LogError($"UpdateHx檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var updateSrc = await _db.Histories.FirstOrDefaultAsync(x => x.Borrow.Cid == (Int32)Cid! && x.HistoryId == Req.HistoryId);

            if (updateSrc == null)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"UpdateHx失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                updateSrc.Score = Req.Score;
                updateSrc.Feedback = Req.Feedback;

                var updateChk = (_db.Entry(updateSrc).State == EntityState.Modified);

                await _db.SaveChangesAsync();

                Res.Status = true;
                Res.Message = (updateChk) ? "修改成功" : "查無相關修改";
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"UpdateHx錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #region Msg
        public async Task<MsgQueryRes> QueryMsg(MsgQueryReq Req)
        {
            var Res = new MsgQueryRes();

            var querySrc = _db.Clients.Where(x => x.Guid == Req.Guid).SelectMany(x => x.Notifications);

            if (!await querySrc.AnyAsync())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"QueryMsg失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var querySrcX = querySrc.Skip((Req.Page - 1) * Req.Size).Take(Req.Size);

                var query = querySrcX.Select(x => new MsgInfo
                {
                    NotificationId = x.NotificationId,
                    Message = x.Message,
                    NotificationDate = x.NotificationDate,
                    IsRead = x.IsRead
                });

                var msgList = await query.ToListAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = await querySrc.CountAsync();
                Res.MsgList = msgList;
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"QueryMsg錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }

        public async Task<MsgSaveRes> UpdateMsg(MsgSaveReq Req)
        {
            var Res = new MsgSaveRes();

            var (check, Cid, message) = await InfoSaveChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = message;

                _logX.L1();
                _logO.LogError($"UpdateMsg檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var updateSrc = await _db.Notifications.FirstOrDefaultAsync(x => x.Cid == (Int32)Cid! && x.NotificationId == Req.NotificationId);

            if (updateSrc == null)
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關紀錄";

                _logX.L1();
                _logO.LogError($"UpdateMsg失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                updateSrc.IsRead = (Boolean)Req.IsRead!;

                var updateChk = (_db.Entry(updateSrc).State == EntityState.Modified);

                await _db.SaveChangesAsync();

                Res.Status = true;
                Res.Message = (updateChk) ? "修改成功" : "查無相關修改";
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"UpdateMsg錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #endregion

        #region Search
        public async Task<SearchQueryRes> QueryBookInfo(SearchQueryReq Req)
        {
            var Res = new SearchQueryRes();

            var (check, message) = SearchQueryChk(Req);

            if (!check)
            {
                Res.Status = false;
                Res.StatusCode = "4003";
                Res.Message = message!;

                _logX.L1();
                _logO.LogError($"QueryBookInfo檢查失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            var querySrc = _db.Collections.AsQueryable();

            if (!String.IsNullOrWhiteSpace(Req.Info))
            {
                switch (Req.Kind)
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

                if (!await querySrc.AnyAsync())
                {
                    Res.Status = false;
                    Res.StatusCode = "4004";
                    Res.Message = Req.Kind switch
                    {
                        "title" => "查無相關書名",
                        "author" => "查無相關作者",
                        "publisher" => "查無相關出版社",
                        "isbn" => "查無相關ISBN",
                        _ => "查無相關館藏"
                    };

                    _logX.L1();
                    _logO.LogError($"QueryBookInfo失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                    return Res;
                }
            }
            if (Req.TypeId != null) { querySrc = querySrc.Where(x => x.TypeId == Req.TypeId); }
            if (Req.LangId != null) { querySrc = querySrc.Where(x => x.LanguageId == Req.LangId); }
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

            if (!await querySrc.AnyAsync())
            {
                Res.Status = false;
                Res.StatusCode = "4004";
                Res.Message = "查無相關館藏";

                _logX.L1();
                _logO.LogError($"QueryBookInfo失敗 - StatusCode = {Res.StatusCode}, Message = {Res.Message}");

                return Res;
            }

            try
            {
                var query = querySrc.Select(x => new BookInfo
                {
                    CollectionId = x.CollectionId,
                    Title = x.Title,
                    BDesc = x.Desc,
                    Image = x.Image,
                    Type = x.Type.Type1,
                    AuthorInfos = x.Authors.Select(y => new AuthorInfo { Author = y.Author1, ADesc = y.Desc }),
                    Translator = x.Translator,
                    Publisher = x.Publisher,
                    Language = x.Language.Language1,
                    ISBN = x.Isbn,
                    PublishDate = x.PublishDate,
                    BookStatus = x.Books.Any(y => (y.BookStatusId == 1) && (!y.Reservations.Any(z => z.ReservationStatusId == 1 || z.ReservationStatusId == 3)))
                });

                var bookInfo = await query.FirstOrDefaultAsync();

                Res.Status = true;
                Res.StatusCode = "2000";
                Res.Message = "查詢成功";
                Res.TotalCount = 1;
                Res.BookInfo = bookInfo;
            }
            catch (Exception ex)
            {
                Res.Status = false;
                Res.StatusCode = "5102";
                Res.Message = $"System Error: {ex.Message}";

                _logX.L1();
                _logO.LogError(ex, $"QueryBookInfo錯誤 - StatusCode = {Res.StatusCode}, Message = {Res.Message}, ex = ");
            }

            return Res;
        }
        #endregion

        #endregion

        #region Aux Methods

        #region Home

        #endregion

        #region Collection
        private (Boolean check, String? message) CollectionQueryChk(CollectionQueryReq model)
        {
            if (model.SYear > model.EYear) { return (false, "Logic Error: 年份 (起) > 年份 (迄)"); }

            return (true, null);
        }
        #endregion

        #region Info
        private async Task<(Boolean check, Int32? Cid, String? message)> InfoSaveChk(InfoSaveReqBase model)
        {
            var querySrc = _db.Clients.Where(x => x.Guid == model.Guid);

            if (!await querySrc.AnyAsync()) { return (false, null, "查無相關使用者"); }

            var query = await querySrc.Select(x => x.Cid).FirstAsync();

            return (true, query, null);
        }

        private async Task<(Boolean check, String? message)> RsvChk(Int32 Cid)
        {
            var querySrc = _db.Reservations.Where(x => x.Cid == Cid && (x.ReservationStatusId == 1 || x.ReservationStatusId == 3));

            if (await querySrc.CountAsync() >= 3) { return (false, "Logic Error: 預約數已達上限"); }

            return (true, null);
        }
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