using MongoDB.Bson;

namespace English.Net8.Api.Models
{
    public abstract class Entity
    {
        public ObjectId Id { get; set; }
        protected Entity()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
