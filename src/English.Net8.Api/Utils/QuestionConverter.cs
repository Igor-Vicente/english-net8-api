using English.Net8.Api.Dtos.Question;
using English.Net8.Api.Models;

namespace English.Net8.Api.Utils
{
    public static class QuestionConverter
    {
        public static Question ToQuestion(AddQuestionDto questionDto)
        {
            return new Question
            {
                Alternatives = questionDto.Alternatives,
                CreatedAt = DateTime.UtcNow,
                Difficulty = questionDto.Difficulty,
                Explanation = questionDto.Explanation,
                Header = questionDto.Header,
                RightAnswer = questionDto.RightAnswer,
                Subtopics = questionDto.Subtopics,
                Topic = questionDto.Topic,
                Type = questionDto.Type,
            };
        }

        public static OutQuestionDto ToOutQuestionDto(Question question)
        {
            return new OutQuestionDto
            {
                Alternatives = question.Alternatives,
                CreatedAt = question.CreatedAt,
                Difficulty = question.Difficulty,
                Explanation = question.Explanation,
                Header = question.Header,
                RightAnswer = question.RightAnswer,
                Subtopics = question.Subtopics,
                Topic = question.Topic,
                Type = question.Type,
                Id = question.Id.ToString()
            };
        }
    }
}
