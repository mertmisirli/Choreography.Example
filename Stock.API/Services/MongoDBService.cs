using MongoDB.Driver;

namespace Stock.API.Services
{
    public class MongoDBService
    {
        readonly IMongoDatabase database;

        public MongoDBService(IConfiguration configuration) 
        {
            MongoClient client = new(configuration.GetConnectionString("MongoDb"));

            database = client.GetDatabase("StockAPIDB");
        }

        public IMongoCollection<T> GetCollection<T>() => database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
    }
}
