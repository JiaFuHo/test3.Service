namespace test3.Dto.Common
{
    public abstract class QueryReqBase
    {
        public Int32 Page { get; set; } = 1;
        public Int32 Size { get; set; } = 10;
        public String? Sort { get; set; } = "";
        public String? Mode { get; set; } = "A";
    }
}