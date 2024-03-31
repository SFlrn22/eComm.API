using eComm.DOMAIN.DTO;
using eComm.INFRASTRUCTURE.Contracts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace eComm.INFRASTRUCTURE.Implementations
{
    public class ExternalDepRepository : IExternalDepRepository
    {
        private readonly HttpClient _httpClient;
        public ExternalDepRepository()
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8000") };
        }

        public async Task<ProductDTO> GetProductFromVoiceRecord(IFormFile file)
        {
            var fileContent = new StreamContent(file.OpenReadStream());

            var contentToUpload = new MultipartFormDataContent();
            contentToUpload.Add(fileContent, "file", file.FileName);

            HttpResponseMessage response = await _httpClient.PostAsync($"/VTT", contentToUpload);


            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            string isbnList = JsonConvert.DeserializeObject<string>(content)!;
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetRecommendedItemsForId(string id, string type)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/GetRecommendations/?isbn={id}&type={type}");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            List<string> result = JsonConvert.DeserializeObject<List<string>>(content)!;
            return result;
        }

        public async Task<List<string>> GetTopTen()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/GetTopTen");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            List<string> isbnList = JsonConvert.DeserializeObject<List<string>>(content)!;
            return isbnList;
        }
    }
}
