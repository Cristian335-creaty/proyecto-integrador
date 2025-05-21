namespace Estudiante.Services
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class Judge0Service
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://judge0-ce.p.rapidapi.com";

        public Judge0Service()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);

            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", "cfebc7cc80mshb3a6fe08062dbeap121459jsn1656ac9ffaba");
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "judge0-ce.p.rapidapi.com");
        }
        /*
        public async Task<string> EnviarCodigoAsync(string codigo, string entrada)
        {
            var payload = new
            {
                source_code = codigo,
                stdin = entrada,
                language_id = 62 // 62 corresponde a Java en Judge0
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/submissions?base64_encoded=false&wait=true", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        */
        public async Task<string> EnviarCodigoAsync(string codigo, string entrada, int languageId)
        {
            var payload = new
            {
                source_code = codigo,
                stdin = entrada,
                language_id = languageId
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/submissions?base64_encoded=false&wait=true", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

    }
}
