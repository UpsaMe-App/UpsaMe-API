using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace UpsaMe_API.Helpers
{
    public class OneSignalOptions
    {
        public string AppId { get; set; } = string.Empty;        // public id OK to store in config
        public string RestApiKey { get; set; } = string.Empty;   // SECRET - put in secrets or env
    }

    public class OneSignalHelper
    {
        private readonly OneSignalOptions _opt;
        private readonly HttpClient _http;

        public OneSignalHelper(IConfiguration cfg, HttpClient http)
        {
            _opt = cfg.GetSection("OneSignal").Get<OneSignalOptions>() ?? new OneSignalOptions();
            _http = http;
        }

        public async Task SendToDeviceAsync(string playerId, string title, string body, object? data = null)
        {
            var payload = new
            {
                app_id = _opt.AppId,
                include_player_ids = new[] { playerId },
                headings = new { en = title },
                contents = new { en = body },
                data = data
            };

            var msg = JsonSerializer.Serialize(payload);
            var req = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
            {
                Content = new StringContent(msg, Encoding.UTF8, "application/json")
            };
            req.Headers.Add("Authorization", $"Basic {_opt.RestApiKey}");

            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }
    }
}