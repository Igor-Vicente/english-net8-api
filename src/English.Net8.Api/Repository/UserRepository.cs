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

        public async Task UpdateAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update.Combine();

            if (!string.IsNullOrEmpty(user.Name))
                update = update.Set(u => u.Name, user.Name);

            if (user.BirthDate.HasValue)
                update = update.Set(u => u.BirthDate, user.BirthDate);

            if (!string.IsNullOrEmpty(user.Phone))
                update = update.Set(u => u.Phone, user.Phone);

            if (!string.IsNullOrEmpty(user.Bio))
                update = update.Set(u => u.Bio, user.Bio);

            if (!string.IsNullOrEmpty(user.City))
                update = update.Set(u => u.City, user.City);

            if (!string.IsNullOrEmpty(user.ContactMeOn))
                update = update.Set(u => u.ContactMeOn, user.ContactMeOn);

            if (!string.IsNullOrEmpty(user.Hobbies))
                update = update.Set(u => u.Hobbies, user.Hobbies);

            if (update != Builders<User>.Update.Combine())
                await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateUserLocationAsync(User user, Location newLocation)
        {
            if (user.Location != newLocation)
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                var update = Builders<User>.Update.Set(u => u.Location, newLocation);
                await _collection.UpdateOneAsync(filter, update);
            }
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

        public async Task<DeleteResult> DeleteUserByIdAsync(ObjectId userId)
        {
            var filter = Builders<User>.Filter.Eq("_id", userId);
            return await _collection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<UserWithDistance>> GetClosestUsersAsync(Location location, ObjectId id, int pageNumber, int pageSize)
        {
            var geoNearStage = new BsonDocument
            {
                { "$geoNear", new BsonDocument
                    {
                        { "near", new BsonDocument
                            {
                                { "type", "Point" },
                                { "coordinates", new BsonArray(new[] { location.Coordinates.Latitude, location.Coordinates.Longitude }) }
                            }
                        },
                        { "distanceField", "distance" },
                        { "spherical", true },
                        { "query", new BsonDocument("_id", new BsonDocument("$ne", id)) }
                    }
                }
            };

            var skipStage = new BsonDocument
            {
                { "$skip", (pageNumber - 1) * pageSize }
            };

            var limitStage = new BsonDocument
            {
                { "$limit", pageSize }
            };

            var pipeline = new[] { geoNearStage, skipStage, limitStage };

            var results = await _collection.Aggregate<UserWithDistance>(pipeline).ToListAsync();
            return results;
        }

        public async Task<IEnumerable<UserWithDistance>> GetMostDistantUsersAsync(Location location, ObjectId id, int pageNumber, int pageSize)
        {
            var geoNearStage = new BsonDocument
            {
                { "$geoNear", new BsonDocument
                    {
                        { "near", new BsonDocument
                            {
                                { "type", "Point" },
                                { "coordinates", new BsonArray(new[] { location.Coordinates.Latitude, location.Coordinates.Longitude }) }
                            }
                        },
                        { "distanceField", "distance" },
                        { "spherical", true },
                        { "query", new BsonDocument("_id", new BsonDocument("$ne", id)) }
                    }
                }
            };

            var sortStage = new BsonDocument
            {
                { "$sort", new BsonDocument("distance", -1) }  // Sort by distance in descending order
            };

            var skipStage = new BsonDocument
            {
                { "$skip", (pageNumber - 1) * pageSize }
            };

            var limitStage = new BsonDocument
            {
                { "$limit", pageSize }
            };

            var pipeline = new[] { geoNearStage, sortStage, skipStage, limitStage };

            var results = await _collection.Aggregate<UserWithDistance>(pipeline).ToListAsync();
            return results;
        }
    }
}
