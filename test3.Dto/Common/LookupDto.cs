namespace test3.Dto.Common
{
    public class LangInfo
    {
        public Byte? LangId { get; set; }
        public String? Lang { get; set; }
    }

    public class SeriesInfo
    {
        public Byte? SeriesId { get; set; }
        public String? Series { get; set; }
    }

    public class TypeInfo
    {
        public Byte? TypeId { get; set; }
        public String? Type { get; set; }
    }

    public class LookupRes : ResBase
    {
        public IEnumerable<TypeInfo>? TypeList { get; set; }
        public IEnumerable<String>? PublisherList { get; set; }
        public IEnumerable<LangInfo>? LangList { get; set; }
        public IEnumerable<SeriesInfo>? SeriesList { get; set; }
    }
}