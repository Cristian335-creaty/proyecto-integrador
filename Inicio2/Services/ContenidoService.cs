using Inicio2.Models;

namespace Inicio2.Services
{
    public class ContenidoService : IContenidoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:44345/api/contenidos";

        public ContenidoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Contenido>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Contenido>>(_baseUrl);
        }

        public async Task<Contenido> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Contenido>($"{_baseUrl}/{id}");
        }

        public async Task CreateAsync(Contenido contenido)
        {
            await _httpClient.PostAsJsonAsync(_baseUrl, contenido);
        }

        public async Task UpdateAsync(Contenido contenido)
        {
            await _httpClient.PutAsJsonAsync($"{_baseUrl}/{contenido.Id}", contenido);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
        }
    }
}
