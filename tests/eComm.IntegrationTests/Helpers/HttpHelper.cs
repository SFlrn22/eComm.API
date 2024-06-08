using Newtonsoft.Json;
using System.Text;

namespace eComm.IntegrationTests.Helpers
{
    public class HttpHelper
    {
        public static StringContent GetJsonHttpContent(object items)
        {
            return new StringContent(JsonConvert.SerializeObject(items), Encoding.UTF8, "application/json");
        }

        public static class Urls
        {
            public const string AUTH_LOGIN = "/api/login";
            public const string GET_PRODUCTS = "/api/GetProducts";
        }
    }
}
