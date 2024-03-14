using eComm.INFRASTRUCTURE.Contracts;
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

        public async Task<List<string>> GetRecommendedItemsForId(string id, string type)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/GetRecommendations/?id={id}&type={type}");
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
