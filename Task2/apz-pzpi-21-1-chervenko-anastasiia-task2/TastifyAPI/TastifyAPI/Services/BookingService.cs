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

        public async Task CreateAsync(Booking newBooking) =>
            await _bookingCollection.InsertOneAsync(newBooking);

        public async Task UpdateAsync(string id, Booking updatedBooking) =>
            await _bookingCollection.ReplaceOneAsync(x => x.Id == id, updatedBooking);

        public async Task DeleteAsync(string id) =>
            await _bookingCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Booking>> GetAllBookingsAsync(string guestId) =>
            await _bookingCollection.Find(x => x.GuestId == guestId).ToListAsync();

        public async Task<Booking> GetByDateAsync(DateTime date) =>
            await _bookingCollection.Find(x => x.DateTime == date).FirstOrDefaultAsync();

        public async Task<List<Booking>> GetSortedByDateAsync(DateTime date)
        {
            var sortDefinition = Builders<Booking>.Sort.Ascending(x => x.DateTime);
            return await _bookingCollection.Find(_ => true)
                .Sort(sortDefinition)
                .ToListAsync();
        }

    }
}
