namespace test3.Dto.Guest
{
    #region Home

    #endregion

    #region Collection
    public class CollectionQueryReq { }

    public class CollectionQueryRes : QueryResBase { }
    #endregion

    #region Search
    public class SearchQueryReq
    {
        public String? Type1 { get; set; }
        public String? Info { get; set; }
        public Int32? SYear { get; set; }
        public Int32? EYear { get; set; }
        public Int32? Lang { get; set; }
        public Int32? Type2 { get; set; }
    }

    public class SearchQueryRes : QueryResBase
    {
        public BookInfo? BookInfo { get; set; }
    }

    public class BookInfo
    {
        public String? Title { get; set; }
        public String? Desc { get; set; }
        public Byte[]? Image { get; set; }
        public String? Type { get; set; }
        public String? Translator { get; set; }
        public String? Publisher { get; set; }
        public String? Language { get; set; }
        public String? ISBN { get; set; }
        public DateTime? PublishDate { get; set; }
    }
    #endregion
}