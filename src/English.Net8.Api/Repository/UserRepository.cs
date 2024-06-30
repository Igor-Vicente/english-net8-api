using English.Net8.Api.Configuration;
using English.Net8.Api.Models;
using English.Net8.Api.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace English.Net8.Api.Repository
{

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IOptions<MongoSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var db = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _collection = db.GetCollection<User>(mongoSettings.Value.UsersCollection);
        }

        public async Task InsertAsync(User user)
        {
            await _collection.InsertOneAsync(user);
        }

        public async Task<User> FindByIdAsync(ObjectId id)
        {
            return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task ReplaceAsync(User user)
        {
            await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task UpdateUserLocationAsync(ObjectId userId, Location newLocation)
        {
            var userDb = await _collection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (userDb.Location != newLocation)
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
                var update = Builders<User>.Update.Set(u => u.Location, newLocation);
                await _collection.UpdateOneAsync(filter, update);
            }
        }

        public async Task<IEnumerable<User>> GetClosestUsersAsync(Location location, ObjectId id, int pageNumber, int pageSize)
        {
            var filterDefinition = Builders<User>.Filter.NearSphere(u => u.Location, location.Coordinates.Latitude, location.Coordinates.Longitude)
                & Builders<User>.Filter.Ne(u => u.Id, id);

            return await _collection
                .Find(filterDefinition)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize)
        {
            var sortDefinition = Builders<User>.Sort.Ascending(u => u.Name);

            return await _collection
                .Find(_ => true)
                .Sort(sortDefinition)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }
    }
}
