using eComm.APPLICATION.Contracts;
using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using eComm.DOMAIN.Utilities;
using eComm.PERSISTENCE.Contracts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace eComm.INFRASTRUCTURE.Implementations
{
    public class ExternalDepRepository : IExternalDepRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IProductRepository _productRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        public ExternalDepRepository(IProductRepository productRepository, IHttpClientFactory httpClientFactory)
        {
            _productRepository = productRepository;
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("Wrapper");
        }

        public async Task<string> GetImageFromText(string title)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/TextToImage/?title={title}");

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            string base64Image = JsonConvert.DeserializeObject<string>(content)!;

            return base64Image.Trim();
        }

        public async Task<ProductDTO> GetProductFromImage(IFormFile file)
        {
            var fileContent = new StreamContent(file.OpenReadStream());

            var contentToUpload = new MultipartFormDataContent();
            contentToUpload.Add(fileContent, "file", file.FileName);

            HttpResponseMessage response = await _httpClient.PostAsync($"/SearchByImage", contentToUpload);

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            string url = JsonConvert.DeserializeObject<string>(content)!;

            Product product = await _productRepository.GetProductByUrlM(url);

            return product.MapProductToDTO();
        }

        public async Task<List<ProductDTO>> GetProductFromVoiceRecord(IFormFile file)
        {
            var fileContent = new StreamContent(file.OpenReadStream());

            var contentToUpload = new MultipartFormDataContent();
            contentToUpload.Add(fileContent, "file", file.FileName);

            HttpResponseMessage response = await _httpClient.PostAsync($"/VTT", contentToUpload);

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            string title = JsonConvert.DeserializeObject<string>(content)!;
            List<Product> products = await _productRepository.GetProductsByName(title);

            return products.MapProductsToDTO();
        }

        public async Task<List<string>> GetRecommendedItemsForId(string id, string type)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/GetRecommendations/?isbn={id}&type={type}");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            List<string> result = JsonConvert.DeserializeObject<List<string>>(content)!;
            return result;
        }

        public async Task<string> GetTextFromImage(IFormFile file)
        {
            var fileContent = new StreamContent(file.OpenReadStream());

            var contentToUpload = new MultipartFormDataContent();
            contentToUpload.Add(fileContent, "file", file.FileName);

            HttpResponseMessage response = await _httpClient.PostAsync($"/ImageToText", contentToUpload);

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            string result = JsonConvert.DeserializeObject<string>(content)!;

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
