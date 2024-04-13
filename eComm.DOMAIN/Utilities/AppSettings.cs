namespace eComm.DOMAIN.Utilities
{
    public class AppSettings
    {
        public const string Name = "AppSettings";
        public JWT Jwt { get; set; } = new();
        public StripeConfig StripeConfiguration { get; set; } = new();
    }

    public partial class JWT
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }

    public partial class StripeConfig
    {
        public string Key { get; set; } = string.Empty;
    }
}
