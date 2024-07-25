using English.Net8.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace English.Net8.Api.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FindByIdAsync(ObjectId id);
        Task<IEnumerable<User>> GetAllAsync(int pageNumber, int pageSize);
        Task InsertAsync(User user);
        Task ReplaceAsync(User user);
        Task UpdateUserLocationAsync(ObjectId userId, Location newLocation);
        Task<IEnumerable<User>> GetClosestUsersAsync(Location location, ObjectId id, int pageNumber, int pageSize);
        Task<DeleteResult> DeleteUserByIdAsync(ObjectId userId);
    }
}
