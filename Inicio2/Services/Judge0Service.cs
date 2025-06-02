namespace Estudiante.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    
    /// Service for interacting with the Judge0 API to evaluate code submissions.
    
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

        
        /// Sends code to Judge0 for evaluation and returns the raw JSON response.
        
        public async Task<string> SendCodeAsync(string code, string input, int languageId)
        {
            var payload = new
            {
                source_code = code,
                stdin = input,
                language_id = languageId
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/submissions?base64_encoded=false&wait=true", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}