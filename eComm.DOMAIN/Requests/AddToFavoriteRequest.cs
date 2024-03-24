namespace eComm.DOMAIN.Requests
{
    public class AddToFavoriteRequest
    {
        public int UserID { get; set; }
        public string ISBN { get; set; } = default!;
    }
}
