namespace eComm.DOMAIN.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime RefreshExpireDate { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
