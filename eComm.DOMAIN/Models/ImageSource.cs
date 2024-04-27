namespace eComm.DOMAIN.Models
{
    public class ImageSource
    {
        public string ImageUrl { get; set; } = string.Empty;
        public List<ReverseImageResult> ResultList { get; set; } = default!;
    }
}
