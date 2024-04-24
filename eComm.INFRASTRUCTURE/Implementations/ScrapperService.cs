using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eComm.APPLICATION.Implementations
{
    public class ScrapperService : IScrapperService
    {
        public async Task<ScrappedData> GetCatAndDesc(string isbn)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}";
            try
            {
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
            catch (Exception ex)
            {
                return new ScrappedData();
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
                return prices;
            }
            return prices;
        }

        public async Task<string> GetImageSource(IFormFile file)
        {
            string url = $"https://lens.google.com/uploadbyurl?url=http://images.amazon.com/images/P/0195153448.01.LZZZZZZZ.jpg&hl=en";

            var fileContent = new StreamContent(file.OpenReadStream());

            var contentToUpload = new MultipartFormDataContent();

            using (HttpClient client = new HttpClient())
            {
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    requestMessage.Headers.Add("User-agent", "Mozilla/5.0 (X11; Linux x86_64; rv:103.0) Gecko/20100101 Firefox/103.0");
                    var result = await client.SendAsync(requestMessage);
                    string contentRes = await result.Content.ReadAsStringAsync();
                }

                HttpResponseMessage response = await client.PostAsync(url, contentToUpload);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                string resp = JsonConvert.DeserializeObject<string>(content)!;
            }

            return "";

        }
    }
}
