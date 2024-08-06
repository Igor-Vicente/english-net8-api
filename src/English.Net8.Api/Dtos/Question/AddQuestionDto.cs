using English.Net8.Api.Models;
using English.Net8.Api.Utils;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Question
{
    public class AddQuestionDto
    {
        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Header { get; set; } = string.Empty;

        [Required]
        [EnsureArraySize(1, 5, ErrorMessage = "The Alternatives array must contain between 1 and 5 elements.")]
        public Alternative[] Alternatives { get; set; } = [];

        [Required]
        [Range(1, 3)]
        public EnglishLevel Difficulty { get; set; }

        [Required]
        [Range(1, 1)]
        public QuestionType Type { get; set; }

        [Required]
        public int RightAnswer { get; set; }

        [Required]
        public string Explanation { get; set; } = string.Empty;

        [Required]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [EnsureArraySize(1, 5, ErrorMessage = "The Alternatives array must contain between 1 and 5 elements.")]
        public string[] Subtopics { get; set; } = [];
    }
}
