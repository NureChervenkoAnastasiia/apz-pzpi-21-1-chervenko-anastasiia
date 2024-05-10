using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class StaffService
    {
        private readonly IMongoCollection<Staff> _staffCollection;

        public StaffService(IMongoDatabase database)
        {
            _staffCollection = database.GetCollection<Staff>("Staff");
        }

        public async Task<List<Staff>> GetAsync() =>
            await _staffCollection.Find(_ => true).ToListAsync();

        public async Task<Staff> GetByIdAsync(string id) =>
            await _staffCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Staff staff) =>
            await _staffCollection.InsertOneAsync(staff);

        public async Task UpdateAsync(string id, Staff updatedStaff) =>
            await _staffCollection.ReplaceOneAsync(x => x.Id == id, updatedStaff);

        public async Task RemoveAsync(string id) =>
            await _staffCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<bool> AnyAsync(Expression<Func<Staff, bool>> filter) =>
            await _staffCollection.Find(filter).AnyAsync();        

        public async Task<Staff> GetByLoginAsync(string login) =>
            await _staffCollection.Find(x => x.Login == login).FirstOrDefaultAsync();
    }
}
