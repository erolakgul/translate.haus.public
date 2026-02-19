using translate.haus.api.Settings;
using Microsoft.Extensions.Options;
using System.Text;
using translate.haus.api.Models;
using System.Text.Json;
using System.Text.Encodings.Web;
using translate.haus.api.Services.interfaces;

namespace translate.haus.api.Services
{
    public class DeeplServices : IDeepLServices
    {
        private readonly DeepLSettings _deepLSettings;

        public DeeplServices(IOptions<DeepLSettings> deepLOptions)
        {
            _deepLSettings = deepLOptions.Value;
        }

        public async Task<TranslationResponse> RequestAsync(TranslationRequests translationRequests)
        {
            var apiKey = _deepLSettings.ApiKey;
            var url = _deepLSettings.EndPoint;

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };

            var json = JsonSerializer.Serialize(translationRequests, options);

            string result = String.Empty;   

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"DeepL-Auth-Key {apiKey}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                result = await response.Content.ReadAsStringAsync();
            }

            TranslationResponse? translations = JsonSerializer.Deserialize<TranslationResponse>(result);

            return translations;
        }

    }
}
