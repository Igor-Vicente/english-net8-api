using English.Net8.Api.Configuration;
using English.Net8.Api.Dtos.Question;
using English.Net8.Api.Models;
using English.Net8.Api.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace English.Net8.Api.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ILogger<QuestionRepository> _logger;
        private readonly IMongoCollection<Question> _questionsCollection;
        private readonly IMongoCollection<QuestionTopics> _topicsCollection;
        private readonly IMongoCollection<UserAnswer> _userAnswersCollection;

        public QuestionRepository(IOptions<MongoSettings> mongoSettings, ILogger<QuestionRepository> logger)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var db = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _questionsCollection = db.GetCollection<Question>(mongoSettings.Value.QuestionsCollection);
            _topicsCollection = db.GetCollection<QuestionTopics>(mongoSettings.Value.QuestionTopicsCollection);
            _userAnswersCollection = db.GetCollection<UserAnswer>(mongoSettings.Value.UserAnswersCollection);
            _logger = logger;
        }
        public async Task<OutQuestionDto> GetFilteredQuestion(ObjectId userId, FilterQuestionDto filter, IEnumerable<ObjectId> seenQuestionIds)
        {
            var filterDefinition = Builders<Question>.Filter.And(
                Builders<Question>.Filter.Eq(q => q.Topic, filter.Topic),
                Builders<Question>.Filter.AnyEq(q => q.Subtopics, filter.Subtopic),
                Builders<Question>.Filter.Eq(q => q.Difficulty, filter.Difficulty)
            );

            if (filter.AlreadyAnswered)
            {
                var userAnswers = await _userAnswersCollection
                    .Find(ua => ua.UserId == userId)
                    .ToListAsync();

                var answeredQuestionIds = userAnswers.Select(ua => ua.QuestionId).ToList();

                filterDefinition &= Builders<Question>.Filter.In(q => q.Id, answeredQuestionIds);
                filterDefinition &= Builders<Question>.Filter.Nin(q => q.Id, seenQuestionIds);

                _logger.LogInformation(filterDefinition.ToString());
                var question = await _questionsCollection.Find(filterDefinition).FirstOrDefaultAsync();

                if (question == null) return null;

                var userAnswer = userAnswers.FirstOrDefault(ua => ua.QuestionId == question.Id);

                return new OutQuestionDto
                {
                    Id = question.Id.ToString(),
                    Header = question.Header,
                    Alternatives = question.Alternatives,
                    Difficulty = question.Difficulty,
                    Type = question.Type,
                    CreatedAt = question.CreatedAt,
                    RightAnswer = question.RightAnswer,
                    Explanation = question.Explanation,
                    Topic = question.Topic,
                    Subtopics = question.Subtopics,
                    UserAnswerId = userAnswer!.UserAnswerId,
                    AnsweredAt = userAnswer!.AnsweredAt
                };
            }
            else
            {
                var answeredQuestionIds = await _userAnswersCollection
                    .Find(ua => ua.UserId == userId)
                    .Project(ua => ua.QuestionId)
                    .ToListAsync();

                filterDefinition &= Builders<Question>.Filter.Nin(q => q.Id, answeredQuestionIds.Concat(seenQuestionIds));

                var question = await _questionsCollection.Find(filterDefinition).FirstOrDefaultAsync();

                if (question == null) return null;

                return new OutQuestionDto
                {
                    Id = question.Id.ToString(),
                    Header = question.Header,
                    Alternatives = question.Alternatives,
                    Difficulty = question.Difficulty,
                    Type = question.Type,
                    CreatedAt = question.CreatedAt,
                    RightAnswer = question.RightAnswer,
                    Explanation = question.Explanation,
                    Topic = question.Topic,
                    Subtopics = question.Subtopics,
                };
            }
        }

        public async Task<IEnumerable<QuestionTopics>> GetAllTopics()
        {
            var sortDefinition = Builders<QuestionTopics>.Sort.Ascending(t => t.Topic);

            return await _topicsCollection.Find(_ => true).Sort(sortDefinition).ToListAsync();
        }

        public async Task AddUserAnswer(UserAnswer userAnswers)
        {
            await _userAnswersCollection.InsertOneAsync(userAnswers);
        }

        public async Task<DeleteResult> DeleteUserAnswersByUserIdAsync(ObjectId userId)
        {
            var filter = Builders<UserAnswer>.Filter.Eq(ua => ua.UserId, userId);
            return await _userAnswersCollection.DeleteManyAsync(filter);
        }

        public async Task<bool> CheckUserAlreadyAnswerTheQuestion(ObjectId userId, ObjectId questionId)
        {
            var filter = Builders<UserAnswer>.Filter.And(
                Builders<UserAnswer>.Filter.Eq(ua => ua.UserId, userId),
                Builders<UserAnswer>.Filter.Eq(ua => ua.QuestionId, questionId)
            );

            var result = await _userAnswersCollection.Find(filter).Limit(1).FirstOrDefaultAsync();
            return result != null;
        }

        public async Task AddNewQuestionAsync(Question question)
        {
            question.Topic = question.Topic.ToLower();
            question.Subtopics = question.Subtopics.Select(s => s.ToLower()).ToArray();

            var existingTopic = await _topicsCollection.Find(t => t.Topic == question.Topic).FirstOrDefaultAsync();
            if (existingTopic == null)
            {
                var newTopic = new QuestionTopics
                {
                    Topic = question.Topic,
                    Subtopics = question.Subtopics
                };
                await _topicsCollection.InsertOneAsync(newTopic);
            }
            else
            {
                var updateDefinition = Builders<QuestionTopics>.Update.AddToSetEach(t => t.Subtopics, question.Subtopics);
                await _topicsCollection.UpdateOneAsync(t => t.Topic == question.Topic, updateDefinition);
            }

            await _questionsCollection.InsertOneAsync(question);
        }
    }
}
