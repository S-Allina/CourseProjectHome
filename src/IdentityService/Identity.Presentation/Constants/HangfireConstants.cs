namespace Identity.Presentation.Constants
{
    public static class HangfireConstants
    {
        public static readonly TimeSpan CommandTimeout = TimeSpan.FromMinutes(5);
        public const bool PrepareSchema = true;
        public const string HangfireConnection = "HangfireConnection";
        public const string HangfireServerName = "MyHangfireServer";
    }
}
