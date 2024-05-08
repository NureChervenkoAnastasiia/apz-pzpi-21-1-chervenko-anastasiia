using MongoDB.Driver;

namespace TastifyAPI.Data
{
    public class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? _database;
        public MongoDbService(IConfiguration configuration) 
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("TastifyDbConnection");
            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase? Database => _database;
    }
}
