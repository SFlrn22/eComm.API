namespace eComm.DOMAIN.DTO
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
