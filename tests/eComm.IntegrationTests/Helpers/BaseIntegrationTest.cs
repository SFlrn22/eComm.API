using eComm.API.ApiMarker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace eComm.IntegrationTests.Helpers
{
    public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<IApiMarker>>
    {
        protected readonly HttpClient _client;
        private readonly AuthHelper _authHelper;
        public BaseIntegrationTest(WebApplicationFactory<IApiMarker> factory)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _client = factory.CreateClient();
            _authHelper = new AuthHelper(configuration);
            var token = _authHelper.HandleAuth();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
