namespace eComm.DOMAIN.Requests
{
    public class RateProductRequest
    {
        public string ISBN { get; set; } = default!;
        public int Rating { get; set; }
    }
}
