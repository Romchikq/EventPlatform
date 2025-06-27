using System.Net.Http.Headers;
using System.Text.Json;

namespace EventPlatform.Services
{
    public interface IMapService
    {
        Task<(decimal? latitude, decimal? longitude)> GetCoordinates(string address);
        Task<string> GetMapImageUrl(decimal latitude, decimal longitude, int width = 600, int height = 400);
    }

    public class MapService : IMapService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MapService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<(decimal? latitude, decimal? longitude)> GetCoordinates(string address)
        {
            var apiKey = _configuration["YandexMaps:ApiKey"];
            var url = $"https://geocode-maps.yandex.ru/1.x/?apikey={apiKey}&format=json&geocode={Uri.EscapeDataString(address)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return (null, null);
            }

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            var featureMember = doc.RootElement
                .GetProperty("response")
                .GetProperty("GeoObjectCollection")
                .GetProperty("featureMember");

            if (featureMember.GetArrayLength() == 0)
            {
                return (null, null);
            }

            var pos = featureMember[0]
                .GetProperty("GeoObject")
                .GetProperty("Point")
                .GetProperty("pos")
                .GetString();

            var coords = pos.Split(' ');
            if (coords.Length != 2 || !decimal.TryParse(coords[1], out var lat) || !decimal.TryParse(coords[0], out var lon))
            {
                return (null, null);
            }

            return (lat, lon);
        }

        public async Task<string> GetMapImageUrl(decimal latitude, decimal longitude, int width = 600, int height = 400)
        {
            var apiKey = _configuration["YandexMaps:ApiKey"];
            return $"https://static-maps.yandex.ru/1.x/?l=map&pt={longitude},{latitude},pm2rdl&size={width},{height}&apikey={apiKey}";
        }
    }
}