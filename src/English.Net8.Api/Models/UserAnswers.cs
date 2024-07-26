using MongoDB.Bson;

namespace English.Net8.Api.Models
{
    public class UserAnswer : Entity
    {
        public ObjectId UserId { get; set; }
        public ObjectId QuestionId { get; set; }
        public int UserAnswerId { get; set; }
        public DateTime AnsweredAt { get; set; }
        public Difficulty QuestionDifficulty { get; set; }
        public bool HasSucceed { get; set; }
    }
}
