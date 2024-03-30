using eComm.APPLICATION.Contracts;
using HtmlAgilityPack;

namespace eComm.APPLICATION.Implementations
{
    public class ScrapperService : IScrapperService
    {
        public List<double> GetPriceFromAmazon(string isbn)
        {

            List<double> prices = new();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"https://webcache.googleusercontent.com/search?client=opera&q=cache%3Ahttps%3A%2F%2Fwww.amazon.com/dp/{isbn}");

            try
            {
                var scrappedData = doc.DocumentNode.SelectSingleNode("//div[@id='tmmSwatches']").SelectNodes("//span[@class='slot-price']");
                foreach (var data in scrappedData)
                {
                    string cleanedPrice = data.InnerText.Trim();
                    double price = Double.Parse(cleanedPrice.Substring(1, cleanedPrice.Length - 1));
                    prices.Add(price);
                }
            }
            catch
            {
                throw;
            }
            return prices;
        }
    }
}
