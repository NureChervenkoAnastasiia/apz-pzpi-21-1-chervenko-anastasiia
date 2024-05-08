using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IStaffService
    {
        Task<List<Staff>> GetAsync();
        Task<Staff?> GetByIdAsync(string id);
        Task CreateAsync(Staff newStaff);
        Task UpdateAsync(string id, Staff updatedStaff);
        Task RemoveAsync(string id);
    }
}
