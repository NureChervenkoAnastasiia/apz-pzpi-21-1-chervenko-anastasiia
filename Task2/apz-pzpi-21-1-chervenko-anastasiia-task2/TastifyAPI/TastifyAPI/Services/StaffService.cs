using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Data;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class StaffService : IStaffService
    {
        private readonly IMongoCollection<Staff> _staffCollection;

        public StaffService(IMongoDatabase database)
        {
            _staffCollection = database.GetCollection<Staff>("Staffs");
        }

        public async Task<List<Staff>> GetAsync() =>
            await _staffCollection.Find(_ => true).ToListAsync();

        public async Task<Staff?> GetByIdAsync(string id) =>
    await _staffCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Staff newStaff) =>
            await _staffCollection.InsertOneAsync(newStaff);

        public async Task UpdateAsync(string id, Staff updatedStaff) =>
            await _staffCollection.ReplaceOneAsync(x => x.Id == id, updatedStaff);

        public async Task RemoveAsync(string id) =>
            await _staffCollection.DeleteOneAsync(x => x.Id == id);
    }
}
