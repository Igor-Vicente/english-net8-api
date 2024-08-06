using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Models
{
    public class Question : Entity
    {
        public string Header { get; set; } = string.Empty;
        public Alternative[] Alternatives { get; set; } = [];
        public EnglishLevel Difficulty { get; set; }
        public QuestionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RightAnswer { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string[] Subtopics { get; set; } = [];
    }

    public class Alternative
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
    }

    public enum QuestionType
    {
        MultipleChoice = 1,
    }

    public enum EnglishLevel
    {
        Basic = 1,
        Intermediate = 2,
        Advanced = 3
    }
}
