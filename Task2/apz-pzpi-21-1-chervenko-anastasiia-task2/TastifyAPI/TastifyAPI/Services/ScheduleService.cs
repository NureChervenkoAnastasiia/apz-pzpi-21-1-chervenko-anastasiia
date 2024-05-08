using MongoDB.Driver;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class ScheduleService
    {
        private readonly IMongoCollection<Schedule> _scheduleCollection;

        public ScheduleService(IMongoDatabase database)
        {
            _scheduleCollection = database.GetCollection<Schedule>("Schedules");
        }

        public async Task<List<Schedule>> GetAsync() =>
            await _scheduleCollection.Find(_ => true).ToListAsync();

        public async Task<Schedule?> GetByIdAsync(string id) =>
            await _scheduleCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Schedule newSchedule) =>
            await _scheduleCollection.InsertOneAsync(newSchedule);

        public async Task UpdateAsync(string id, Schedule updatedSchedule) =>
            await _scheduleCollection.ReplaceOneAsync(x => x.Id == id, updatedSchedule);

        public async Task RemoveAsync(string id) =>
            await _scheduleCollection.DeleteOneAsync(x => x.Id == id);
    }
}
