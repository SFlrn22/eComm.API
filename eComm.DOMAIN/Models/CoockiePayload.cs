using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace eComm.DOMAIN.Models
{
    public class CoockiePayload
    {
        public string gl { get; set; } = "RO";
        public string m { get; set; } = "0";
        public string app { get; set; } = "0";
        public string pc { get; set; } = "l";
        [JsonPropertyName("continue")]
        public string Continue { get; set; } = default!;
        public string x { get; set; } = "6";
        public string bl { get; set; } = "";
        public string hl { get; set; } = "";
        public string src { get; set; } = "1";
        public string cm { get; set; } = "2";
        public string set_eom { get; set; } = "true";

        public string ToQueryString()
        {
            var queryString = new StringBuilder();

            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(this)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    if (queryString.Length > 0)
                        queryString.Append('&');
                    queryString.Append($"{WebUtility.UrlEncode(property.Name)}={WebUtility.UrlEncode(value)}");
                }
            }

            return queryString.ToString();
        }
    }

}
