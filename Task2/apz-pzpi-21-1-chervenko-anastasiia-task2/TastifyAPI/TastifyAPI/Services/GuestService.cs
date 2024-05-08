using MongoDB.Driver;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class GuestService
    {
        private readonly IMongoCollection<Guest> _guestCollection;

        public GuestService(IMongoDatabase database)
        {
            _guestCollection = database.GetCollection<Guest>("Guests");
        }
        public async Task<List<Guest>> GetAsync() =>
            await _guestCollection.Find(_ => true).ToListAsync();

        public async Task<Guest?> GetByIdAsync(string id) =>
            await _guestCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Guest newguest) =>
            await _guestCollection.InsertOneAsync(newguest);

        public async Task UpdateAsync(string id, Guest updatedguest) =>
            await _guestCollection.ReplaceOneAsync(x => x.Id == id, updatedguest);

        public async Task RemoveAsync(string id) =>
            await _guestCollection.DeleteOneAsync(x => x.Id == id);
    }
}
