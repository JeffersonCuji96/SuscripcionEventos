namespace BL.Common
{
    public class AppSettings
    {
        public string SecretKey { get; set; } =string.Empty;
        public int DaysExpireToken { get; set; }
        public string SupportEmail { get; set; } = string.Empty;
        public string SupportPass { get; set; } = string.Empty;
        public string UrlDomain { get; set; } = string.Empty;
    }
}
