using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IMenuService
    {
        Task<List<Menu>> GetAsync();
        Task<Menu?> GetByIdAsync(string id);
        Task CreateAsync(Menu newMenu);
        Task UpdateAsync(string id, Menu updatedMenu);
        Task RemoveAsync(string id);
    }
}
