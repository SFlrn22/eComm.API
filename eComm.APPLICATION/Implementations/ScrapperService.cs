using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Models;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace eComm.APPLICATION.Implementations
{
    public class ScrapperService : IScrapperService
    {
        public async Task<ScrappedData> GetCatAndDesc(string isbn)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();


                string json = await response.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(json);

                string selfLink = data.items[0].selfLink;

                HttpResponseMessage response2 = await client.GetAsync(selfLink);
                string json2 = await response2.Content.ReadAsStringAsync();
                dynamic data2 = JObject.Parse(json2);

                string category = data.items[0].volumeInfo.categories[0];
                string description = data2.volumeInfo.description;

                ScrappedData result = new()
                {
                    Category = category,
                    Description = description
                };

                return result;
            }
        }

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
                return new List<double>();
            }
            return prices;
        }
    }
}
