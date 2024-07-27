using English.Net8.Api.Dtos.Question;
using English.Net8.Api.Models;
using MongoDB.Bson;

namespace English.Net8.Api.Repository.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<QuestionTopics>> GetAllTopics();
        Task<OutQuestionDto> GetFilteredQuestion(ObjectId userId, FilterQuestionDto filter, IEnumerable<ObjectId> seenQuestionObjectIds);
        Task AddNewQuestionAsync(Question question);
        Task AddUserAnswer(UserAnswer userAnswers);
        Task<bool> CheckUserAlreadyAnswerTheQuestion(ObjectId userId, ObjectId questionId);
    }
}
