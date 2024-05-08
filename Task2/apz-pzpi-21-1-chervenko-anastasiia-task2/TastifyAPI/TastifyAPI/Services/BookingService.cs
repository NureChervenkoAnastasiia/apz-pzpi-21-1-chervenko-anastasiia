using MongoDB.Driver;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IMongoCollection<Booking> _bookingCollection;

        public BookingService(IMongoDatabase database)
        {
            _bookingCollection = database.GetCollection<Booking>("Bookings");
        }

        public async Task<List<Booking>> GetAsync() =>
            await _bookingCollection.Find(_ => true).ToListAsync();

        public async Task<Booking?> GetByIdAsync(string id) =>
            await _bookingCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Booking newbooking) =>
            await _bookingCollection.InsertOneAsync(newbooking);

        public async Task UpdateAsync(string id, Booking updatedbooking) =>
            await _bookingCollection.ReplaceOneAsync(x => x.Id == id, updatedbooking);

        public async Task RemoveAsync(string id) =>
            await _bookingCollection.DeleteOneAsync(x => x.Id == id);
    }
}
