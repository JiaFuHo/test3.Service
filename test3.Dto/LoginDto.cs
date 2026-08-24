namespace test3.Dto
{
    public class LoginReq
    {
        public String? CAcc { get; set; }
        public String? CPwd { get; set; }
    }

    public class LoginRes : ResBase
    {
        public Int32? CId { get; set; }
        public Guid? Guid { get; set; }
        public String? CName { get; set; }
        public String? CPhone { get; set; }
        public Byte? Permission { get; set; }
    }
}