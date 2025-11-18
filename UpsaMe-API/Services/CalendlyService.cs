using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace UpsaMe_API.Services
{
    public class CalendlyService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        public CalendlyService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _http.BaseAddress = new Uri(config["Calendly:BaseUrl"] ?? "https://api.calendly.com/");
            _apiKey = config["Calendly:ApiKey"] ?? "";
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        // Ejemplo: obtener eventos de un usuario
        public async Task<T?> GetUserEventsAsync<T>(string userUri)
        {
            var resp = await _http.GetAsync(userUri);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<T>();
        }

        // Ejemplo: crear una invitación / scheduling (según Calendly API)
        public async Task<HttpResponseMessage> CreateInviteAsync(object payload)
        {
            var resp = await _http.PostAsJsonAsync("scheduled_events", payload);
            return resp;
        }
    }
}