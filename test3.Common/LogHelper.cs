namespace test3.Common
{
    public static class _logX
    {
        #region Fields
        public static Serilog.ILogger Decorator { get; set; } = null!;
        #endregion

        #region Methods
        public static void L1()
        {
            Decorator.Information("====================================================================================================");
        }

        public static void L2()
        {
            Decorator.Information("----------------------------------------------------------------------------------------------------");
        }
        #endregion
    }
}