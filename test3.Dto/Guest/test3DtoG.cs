using test3.Dto.Common;

namespace test3.Dto.Guest
{
    public class BookInfo
    {
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

    public class AuthorInfo
    {
        public String? Author { get; set; }
        public String? ADesc { get; set; }
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

    public class HomeQuerySeriesRes : QueryResBase
    {
        public IEnumerable<String>? SeriesList { get; set; }
    }
    #endregion

    #region Collection
    public class CollectionQueryReq { }

    public class CollectionQueryRes : QueryResBase { }
    #endregion

    #region Info

    #endregion

    #region Search
    public class SearchQueryReq
    {
        public String? Type1 { get; set; }
        public String? Info { get; set; }
        public Int16? SYear { get; set; }
        public Int16? EYear { get; set; }
        public Byte? Lang { get; set; }
        public Byte? Type2 { get; set; }
    }

    public class SearchQueryRes : QueryResBase
    {
        public BookInfo? BookInfo { get; set; }
    }
    #endregion
}