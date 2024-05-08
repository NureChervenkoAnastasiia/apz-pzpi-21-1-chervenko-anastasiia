using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAsync();
        Task<Booking?> GetByIdAsync(string id);
        Task CreateAsync(Booking newBooking);
        Task UpdateAsync(string id, Booking updatedBooking);
        Task RemoveAsync(string id);
    }
}
