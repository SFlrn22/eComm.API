using eComm.INFRASTRUCTURE.Contracts;

namespace eComm.INFRASTRUCTURE.Implementations
{
    public class ExternalDepRepository : IExternalDepRepository
    {
        private readonly HttpClient _httpClient;
        public ExternalDepRepository()
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri("http://127.0.0.1:8000") };
        }

        public async Task<List<string>> GetTopTen()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/GetTopTen");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            content = content.Substring(1, content.Length - 2);
            List<string> result = content.Split(',').ToList();
            return result;
        }
    }
}
