namespace test3.Dto.Common
{
    public abstract class ResBase
    {
        public Boolean Status { get; set; } = false;
        public String StatusCode { get; set; } = "";
        public String Message { get; set; } = "";
    }

    public abstract class QueryResBase : ResBase
    {
        public Int32 TotalCount { get; set; } = 0;
    }
}