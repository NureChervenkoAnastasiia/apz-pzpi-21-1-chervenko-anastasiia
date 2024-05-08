using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IGuestService
    {
        Task<List<Guest>> GetAsync();
        Task<Guest?> GetByIdAsync(string id);
        Task CreateAsync(Guest newGuest);
        Task UpdateAsync(string id, Guest updatedGuest);
        Task RemoveAsync(string id);
    }
}
