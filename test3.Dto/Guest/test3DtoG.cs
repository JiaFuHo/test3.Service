using test3.Dto.Common;

namespace test3.Dto.Guest
{
    public class AuthorInfo
    {
        public String? Author { get; set; }
        public String? ADesc { get; set; }
    }

    public class BookInfo
    {
        public Int32? CollectionId { get; set; }
        public String? Title { get; set; }
        public String? BDesc { get; set; }
        public Byte[]? Image { get; set; }
        public String? Type { get; set; }
        public IEnumerable<AuthorInfo>? AuthorInfos { get; set; }
        public String? Translator { get; set; }
        public String? Publisher { get; set; }
        public String? Language { get; set; }
        public String? ISBN { get; set; }
        public DateTime? PublishDate { get; set; }
        public Boolean BookStatus { get; set; } = false;
    }

    #region Home
    public class HomeQueryBookReq
    {
        public String? Mode { get; set; }
    }

    public class HomeQueryBookRes : QueryResBase
    {
        public IEnumerable<BookInfo>? BookList { get; set; }
    }
    #endregion

    #region Collection
    public class CollectionQueryReq : QueryReqBase
    {
        public Byte? TypeId { get; set; }
        public String? Publisher { get; set; }
        public Byte? LangId { get; set; }
        public Byte? SeriesId { get; set; }
        public Int16? SYear { get; set; }
        public Int16? EYear { get; set; }
    }

    public class CollectionQueryRes : QueryResBase
    {
        public IEnumerable<BookInfo>? BookList { get; set; }
    }
    #endregion

    #region Info
    public abstract class InfoQueryReqBase : QueryReqBase
    {
        public Guid? Guid { get; set; }
    }

    public abstract class InfoSaveReqBase
    {
        public Guid? Guid { get; set; }
    }

    #region Fav
    public class FavQueryReq : InfoQueryReqBase { }

    public class FavQueryRes : QueryResBase
    {
        public IEnumerable<FavInfo>? FavList { get; set; }
    }

    public class FavInfo
    {
        public Int32? CollectionId { get; set; }
        public String? Title { get; set; }
        public Byte[]? Image { get; set; }
        public IEnumerable<AuthorInfo>? AuthorInfos { get; set; }
        public String? Publisher { get; set; }
    }

    public class FavSaveReq : InfoSaveReqBase
    {
        public Int32? CollectionId { get; set; }
    }

    public class FavSaveRes : ResBase { }
    #endregion

    #region Rsv
    public class RsvQueryReq : InfoQueryReqBase { }

    public class RsvQueryRes : QueryResBase
    {
        public IEnumerable<RsvInfo>? RsvList { get; set; }
    }

    public class RsvInfo
    {
        public Int32? CollectionId { get; set; }
        public String? Title { get; set; }
        public Byte[]? Image { get; set; }
        public IEnumerable<AuthorInfo>? AuthorInfos { get; set; }
        public DateTime? ReservateDate { get; set; }
        public DateTime? DueDateR { get; set; }
        public String? ReservationStatus { get; set; }
    }

    public class RsvSaveReq : InfoSaveReqBase
    {
        public Int32? CollectionId { get; set; }
        public Byte? ReservationStatusId { get; set; }
    }

    public class RsvSaveRes : ResBase { }
    #endregion

    #region Hx
    public class HxQueryReq : InfoQueryReqBase { }

    public class HxQueryRes : QueryResBase
    {
        public IEnumerable<HxInfo>? HxList { get; set; }
    }

    public class HxInfo
    {
        public Int32? HistoryId { get; set; }
        public String? Title { get; set; }
        public Byte[]? Image { get; set; }
        public IEnumerable<AuthorInfo>? AuthorInfos { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? DueDateB { get; set; }
        public String? BorrowStatus { get; set; }
        public Byte? Score { get; set; }
        public String? Feedback { get; set; }
    }

    public class HxSaveReq : InfoSaveReqBase
    {
        public Int32? HistoryId { get; set; }
        public Byte? Score { get; set; }
        public String? Feedback { get; set; }
    }

    public class HxSaveRes : ResBase { }
    #endregion

    #region Msg
    public class MsgQueryReq : InfoQueryReqBase { }

    public class MsgQueryRes : QueryResBase
    {
        public IEnumerable<MsgInfo>? MsgList { get; set; }
    }

    public class MsgInfo
    {
        public Int32? NotificationId { get; set; }
        public String? Message { get; set; }
        public DateTime? NotificationDate { get; set; }
        public Boolean IsRead { get; set; }
    }

    public class MsgSaveReq : InfoSaveReqBase
    {
        public Int32? NotificationId { get; set; }
        public Boolean? IsRead { get; set; }
    }

    public class MsgSaveRes : ResBase { }
    #endregion

    #endregion

    #region Search
    public class SearchQueryReq
    {
        public String? Kind { get; set; }
        public String? Info { get; set; }
        public Int16? SYear { get; set; }
        public Int16? EYear { get; set; }
        public Byte? LangId { get; set; }
        public Byte? TypeId { get; set; }
    }

    public class SearchQueryRes : QueryResBase
    {
        public BookInfo? BookInfo { get; set; }
    }
    #endregion
}