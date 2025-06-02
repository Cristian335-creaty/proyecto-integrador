using Inicio2.Models;

namespace Inicio2.Services
{
    public interface IContenidoService
    {
        Task<List<Contenido>> GetAllAsync();
        Task<Contenido> GetByIdAsync(int id);
        Task CreateAsync(Contenido contenido);
        Task UpdateAsync(Contenido contenido);
        Task DeleteAsync(int id);
    }
}
