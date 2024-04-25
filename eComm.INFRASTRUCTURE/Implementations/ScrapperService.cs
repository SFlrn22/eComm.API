using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
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
                    var htmlContent = await result.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);
                    var popupNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='VtwTSb']").SelectSingleNode("//form[1]");
                    var inputs = popupNode.SelectNodes("//input").Take(11);
                    if (popupNode != null)
                    {
                        var payload = new CoockiePayload()
                        {
                            gl = inputs.ElementAt(0).GetAttributeValue("value", ""),
                            m = inputs.ElementAt(1).GetAttributeValue("value", ""),
                            app = inputs.ElementAt(2).GetAttributeValue("value", ""),
                            pc = inputs.ElementAt(3).GetAttributeValue("value", ""),
                            Continue = inputs.ElementAt(4).GetAttributeValue("value", ""),
                            x = inputs.ElementAt(5).GetAttributeValue("value", ""),
                            bl = inputs.ElementAt(6).GetAttributeValue("value", ""),
                            hl = inputs.ElementAt(7).GetAttributeValue("value", ""),
                            src = inputs.ElementAt(8).GetAttributeValue("value", ""),
                            cm = inputs.ElementAt(9).GetAttributeValue("value", ""),
                            set_eom = inputs.ElementAt(10).GetAttributeValue("value", "")
                        };
                        var jsonContent = System.Text.Json.JsonSerializer.Serialize(payload);

                        var qstring = payload.ToQueryString();

                        var formData = new Dictionary<string, string>
                        {
                            {"gl", "RO"},
                            {"m", "0"},
                            {"app", "0"},
                            {"pc", "l"},
                            {"Continue", inputs.ElementAt(4).GetAttributeValue("value", "")},
                            {"x", "6"},
                            {"bl", inputs.ElementAt(6).GetAttributeValue("value", "")},
                            {"hl", "en"},
                            {"src", "1"},
                            {"cm", "2"},
                            {"set_eom", "true"}
                        };

                        var contentForm = new FormUrlEncodedContent(formData);

                        var stringContent = new StringContent(qstring, null, "application/x-www-form-urlencoded");

                        using (HttpClient client2 = new HttpClient())
                        {
                            try
                            {
                                client2.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                                client2.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
                                client2.DefaultRequestHeaders.Add("cache-control", "max-age=0");
                                client2.DefaultRequestHeaders.Add("cookie", "__Secure-3PAPISID=sOFG4bgjSxezz9YX/ArHahOrlUuGwzLAiZ; __Secure-3PSID=g.a000iwjxgOJay6FR3zwS_IIDipkbwKIp170DTYFFH3QC5GODI8FK2V-xvjm9vbrRzROwlFAbjQACgYKAZ4SAQASFQHGX2Mintg6FcGMNohKkbXW0soyYxoVAUF8yKrXy9aKoYLflhUq5J632Ws40076; NID=513=ZutXBeLTJowKlsBHPh1dym-PUglDzYoAeSiGKO6YarvsoTdtKWeMNSoZZ957QVN5Fa9gTJgQx7GT2lurbjLW2X6bZmpyxafcTOKAeFVPdHitewIYd9UfPmhr4xSXd2FJHjdiR2LnfAo5XYuxLGjh37Ay3OOP1Fo85UQ-zNx7Lc3FPsCmvmmZ25kBUe4Ghu9rNW8cezDb7UkNrRwnH32hStTqEW-_svnYM6gYSNv_3SQseqFXL-J1nlC0IfxAs9mwyn88bVI9gLLAOnrM60899ydA_WxbAbPXDzlv09UxxicA2DJSRP71X3yn_s8uVgRobJ7kzpsCYMa1gb2kngm_haykewS3zmrz_j14lFvexzhrwJM2dvJ_Cce9AenmSqbajPhqh7xO__IU9oT3xFZl0kIA8rz0VlAnSZX7QlW1iuWf8uf9epJ1lKg; __Secure-3PSIDTS=sidts-CjEBLwcBXDxfwOK-GlU5Opghc1tUJXTb2GadEHpvulSjlwKsNNlrqHE8Zs-WGh8Y0ECXEAA; SIDCC=AKEyXzXiB-nhlCR2E-qSj9NCoDjhI73ueXSjkcsB6bfec6TXVhSxYtS1zVuKsxTv_ejeQFYQ-g; __Secure-1PSIDCC=AKEyXzWYf9KtXOPw4tsPZf68qiOLcRP5bmdEmEmMZvrFMvuShClDt0ww53yqNJRVnSK9Dz5d; __Secure-3PSIDCC=AKEyXzV2v5dhgdp7MOE40aVEOo8Sac1GiciQEruIPtLGWkUjsu5ya8hqDJYfC0L3hoY7XMYIObae");
                                client2.DefaultRequestHeaders.Add("origin", "null");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua", "\"Opera\";v=\"109\", \"Not:A-Brand\";v=\"8\", \"Chromium\";v=\"123\"");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-arch", "x86");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-bitness", "64");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-full-version", "109.0.5097.45");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-full-version-list", "\"Opera\";v=\"109.0.5097.45\", \"Not:A-Brand\";v=\"8.0.0.0\", \"Chromium\";v=\"123.0.6312.106\"");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-model", "\"\"");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-platform-version", "15.0.0");
                                client2.DefaultRequestHeaders.Add("sec-ch-ua-wow64", "?0");
                                client2.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
                                client2.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
                                client2.DefaultRequestHeaders.Add("sec-fetch-site", "cross-site");
                                client2.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
                                client2.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
                                client2.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36 OPR/109.0.0.0");
                                client2.DefaultRequestHeaders.Add("x-client-data", "CIuOywE=");
                                var postResponse = await client2.PostAsync("https://consent.google.com/save", stringContent);
                                var content = await postResponse.Content.ReadAsStringAsync();
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                }
            }

            return "";

        }
    }
}
