using English.Net8.Api.Models;
using MongoDB.Bson;

namespace English.Net8.Api.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FindByIdAsync(ObjectId id);
        Task InsertAsync(User user);
        Task ReplaceAsync(User user);
        Task UpdateUserLocationAsync(ObjectId userId, Location newLocation);
        Task<IEnumerable<User>> GetClosestUsersAsync(Location location);
    }
}
