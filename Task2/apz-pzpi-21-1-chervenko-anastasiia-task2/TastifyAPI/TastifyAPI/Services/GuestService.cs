using MongoDB.Driver;
using System.Linq.Expressions;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class GuestService : IGuestService
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

        public async Task<bool> AnyAsync(Expression<Func<Guest, bool>> filter) =>
            await _guestCollection.Find(filter).AnyAsync();

        public async Task<Guest> GetByLoginAsync(string login) =>
            await _guestCollection.Find(guest => guest.Login == login).FirstOrDefaultAsync();

        public async Task CreateAsync(Guest newguest) =>
            await _guestCollection.InsertOneAsync(newguest);

        public async Task UpdateAsync(string id, Guest updatedguest) =>
            await _guestCollection.ReplaceOneAsync(x => x.Id == id, updatedguest);

        public async Task RemoveAsync(string id) =>
            await _guestCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<(int discount, int remainingBonus)> CalculateCouponAsync(int bonus)
        {
            decimal bonusCoefficient = 0.7m;
            int discount = 0;

            if (bonus < 100)
            {
                return (discount, bonus);
            }

            if (bonus <= 200)
            {
                bonusCoefficient = 0.5m;
            }
            else if (bonus <= 300)
            {
                bonusCoefficient = 0.6m;
            }

            discount = (int)(bonus * bonusCoefficient);
            int remainingBonus = bonus - discount;

            return (discount, remainingBonus);
        }

        public async Task<List<Guest>> GetSortedByNameAndBonusAsync()
        {
            var guests = await _guestCollection.Find(_ => true).ToListAsync();
            return guests.OrderBy(g => g.Name).ThenByDescending(g => g.Bonus).ToList();
        }

        
    }
}
