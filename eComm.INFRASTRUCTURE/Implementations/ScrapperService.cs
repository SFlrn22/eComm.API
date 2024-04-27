using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eComm.APPLICATION.Implementations
{
    public class ScrapperService : IScrapperService
    {
        private readonly IExternalDepRepository _externalRepository;
        public ScrapperService(IExternalDepRepository externalRepository)
        {
            _externalRepository = externalRepository;
        }
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

        public async Task<List<ReverseImageResult>> GetImageSource(IFormFile file)
        {
            ProductDTO product = await _externalRepository.GetProductFromImage(file);

            string url = $"https://lens.google.com/uploadbyurl?url={product.ImageUrlL}";

            List<ReverseImageResult> result = new List<ReverseImageResult>();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
                    client.DefaultRequestHeaders.Add("cache-control", "max-age=0");
                    client.DefaultRequestHeaders.Add("cookie", "HSID=AGpLaI-ciyrVqbZX3; SSID=An38ILH2iUkCMrsGj; APISID=z-_0F0rkVCHuLn9Z/ABeXcSUJMeu9crLty; SAPISID=sOFG4bgjSxezz9YX/ArHahOrlUuGwzLAiZ; __Secure-1PAPISID=sOFG4bgjSxezz9YX/ArHahOrlUuGwzLAiZ; __Secure-3PAPISID=sOFG4bgjSxezz9YX/ArHahOrlUuGwzLAiZ; SID=g.a000iwjxgOJay6FR3zwS_IIDipkbwKIp170DTYFFH3QC5GODI8FK7rLUq2aP1HlrA8YoXfje7AACgYKARISAQASFQHGX2MiP4QlrSH4o4-dWIN1uwv_XxoVAUF8yKqqr9qyI8WEr0M4_QI_zZbg0076; __Secure-1PSID=g.a000iwjxgOJay6FR3zwS_IIDipkbwKIp170DTYFFH3QC5GODI8FKqUPzfxWCwBRCGRnmWPHtzgACgYKAdISAQASFQHGX2MieCpIJDv7aA3ddXiEcAEZdxoVAUF8yKrZwM_ZyCwiuY8aEhxdouWa0076; __Secure-3PSID=g.a000iwjxgOJay6FR3zwS_IIDipkbwKIp170DTYFFH3QC5GODI8FK2V-xvjm9vbrRzROwlFAbjQACgYKAZ4SAQASFQHGX2Mintg6FcGMNohKkbXW0soyYxoVAUF8yKrXy9aKoYLflhUq5J632Ws40076; OSID=g.a000iwjxgLLie1Qxt050AMaEfWsCmEG64vF_HJBmqUzhrQyXCxHESvGai45A4PoYTI4UuY8y9gACgYKAUASAQASFQHGX2MisqCtpNkPlfzzQtjf24QJwhoVAUF8yKqy_8p283Gj9W9XmHnrKVbB0076; __Secure-OSID=g.a000iwjxgLLie1Qxt050AMaEfWsCmEG64vF_HJBmqUzhrQyXCxHEcPEZhl-p1A54e1xelpZXGwACgYKAaASAQASFQHGX2MiuLw6pUdUZmC5FQrcn9DWthoVAUF8yKqY0-bjWWIPqsHyZIgyJob_0076; OTZ=7526476_44_48_123900_44_436380; SOCS=CAESNQgQEitib3FfaWRlbnRpdHlmcm9udGVuZHVpc2VydmVyXzIwMjQwNDIxLjA4X3AwGgJlbiACGgYIgLqmsQY; AEC=AQTF6Hxtr4pqKKzX5PD_V4xVnp458UsHcED4JkNQvqARA4he4HREPGYylw; NID=513=bK1oIecPwhblLQGFYIPyqDQtxr0O7jkPzBJxOXTGnPUyT0wGXBX_xwsqLBWLPaA-l_ZtfdphAigd6R0utUYxoSkvMFQMmNLpFPHUUI2Ms-4JCsv99-v3H9KcPvFb7bNanVT6jgdgTUM-lQiFZVbCNzfjU1Ovhs_RiOEWeViqmJ5w0Ozg6Br50xXPxNtF1dOuUoXtb9FCNlr8v_W_mI2GNJCu45iMNWb8ETDI-fBvhL_5dGTqnQGt7WwirKjyFAKIUNAGaPRMlbWNSo9hMGDvsyFGIBOSVifF39zfYCRpcXEhL34YCgM3HJP3nQgUJis7n-kfGXa-cG0SI5-zWy9eznHAAURUW9HdgC3uB48fkYmfLar2V81FhAoW1h5UzUpLBKWEefBtwtMiOoJ2sCquDQIduWPW5ZVJhx9O3QXBH6qE_m0Iq9qAKXs; __Secure-1PSIDTS=sidts-CjEBLwcBXLgt6G5hCcRoAyOa9danrs0PO0wlvnRwwoQjJ8-q2QQRgoWZHsenLObbA_cjEAA; __Secure-3PSIDTS=sidts-CjEBLwcBXLgt6G5hCcRoAyOa9danrs0PO0wlvnRwwoQjJ8-q2QQRgoWZHsenLObbA_cjEAA; __Secure-3PSIDCC=AKEyXzVXcGaph6WVxWH3W9kTHA5GR2Ue7pLssyVKMCqdT7uq4kzkoIuJuVBbZKaFmtXxbZAvbdLL; __Secure-ENID=19.SE=oQ1AZC7cb7raWtyme9aas8YdwLWYerp6mHNkNLoCnrXia4Kd2L-tca7c0t2TvuPuV9BCIgudS9jLtV4__O_nn3gH5qTcHprFsyZ-gYn_umeefS9X4kVJuzVcts1J8yf4If--e8Hw47c1CBbmWbInTLM2LNYSg5MeFeCJ1dtJaiE");
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Opera\";v=\"109\", \"Not:A-Brand\";v=\"8\", \"Chromium\";v=\"123\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-arch", "x86");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-bitness", "64");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-full-version", "109.0.5097.45");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Opera\";v=\"109.0.5097.45\", \"Not:A-Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"123.0.6312.106\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-model", "");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "15.0.0");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-wow64", "?0");
                    client.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
                    client.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
                    client.DefaultRequestHeaders.Add("sec-fetch-site", "cross-site");
                    client.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
                    client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 OPR/109.0.0.0");
                    client.DefaultRequestHeaders.Add("x-client-data", "CIuOywE=");
                    var postResponse = await client.GetAsync(url);
                    var content = await postResponse.Content.ReadAsStringAsync();

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(content);

                    HtmlNode scriptNode = document.DocumentNode.SelectNodes("//script")[18];
                    string nodeContent = scriptNode.InnerHtml;

                    string strippedString = nodeContent.TrimStart("AF_initDataCallback(".ToCharArray()).TrimEnd(");".ToCharArray());
                    dynamic jsonObject = JsonConvert.DeserializeObject(strippedString)!;

                    var data = (((((((((jsonObject.data[1])[0])[1])[8])[8])[0])[12])[0])[9])[0];

                    foreach (var item in data)
                    {
                        ReverseImageResult scrappedItem = new ReverseImageResult()
                        {
                            Link = item[5].Value,
                            Title = item[6].Value
                        };
                        result.Add(scrappedItem);
                    }
                }
                catch (Exception ex)
                {
                    return new List<ReverseImageResult>();
                }
            }

            return result;

        }
    }
}
