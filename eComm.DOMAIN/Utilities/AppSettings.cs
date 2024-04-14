namespace eComm.DOMAIN.Utilities
{
    public class AppSettings
    {
        public const string Name = "AppSettings";
        public JWTConfig JwtConfiguration { get; set; } = new();
        public StripeConfig StripeConfiguration { get; set; } = new();
        public SmtpConfig SmtpConfiguration { get; set; } = new();
    }

    public partial class JWTConfig
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }

    public partial class StripeConfig
    {
        public string Key { get; set; } = string.Empty;
    }
    public partial class SmtpConfig
    {
        public string Address { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
    }
}
