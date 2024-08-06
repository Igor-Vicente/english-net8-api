using English.Net8.Api.Models;

namespace English.Net8.Api.Dtos.Question
{
    public class OutQuestionDto
    {
        public string Id { get; set; }
        public string Header { get; set; }
        public Alternative[] Alternatives { get; set; }
        public EnglishLevel Difficulty { get; set; }
        public QuestionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RightAnswer { get; set; }
        public string Explanation { get; set; }
        public string Topic { get; set; }
        public string[] Subtopics { get; set; }
        public int UserAnswerId { get; set; }
        public DateTime AnsweredAt { get; set; }
    }
}
