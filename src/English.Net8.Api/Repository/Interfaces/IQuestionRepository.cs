using English.Net8.Api.Dtos.Question;
using English.Net8.Api.Models;
using MongoDB.Bson;

namespace English.Net8.Api.Repository.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<QuestionTopics>> GetAllTopics();
        Task<List<OutQuestionDto>> GetFilteredQuestions(ObjectId userId, FilterQuestionDto filter);
        Task AddNewQuestionAsync(Question question);
        Task AddUserAnswer(UserAnswer userAnswers);
        Task<bool> CheckUserAlreadyAnsweredQuestion(ObjectId userId, ObjectId questionId);
    }
}
