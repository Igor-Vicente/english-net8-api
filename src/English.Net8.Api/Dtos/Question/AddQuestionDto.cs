using English.Net8.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Question
{
    public class AddQuestionDto
    {
        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Header { get; set; }

        [EnsureArraySize(1, 5, ErrorMessage = "The Alternatives array must contain between 1 and 5 elements.")]
        public Alternative[] Alternatives { get; set; }

        [Required]
        public Difficulty Difficulty { get; set; }

        [Required]
        public QuestionType Type { get; set; }

        [Required]
        public int RightAnswer { get; set; }

        [Required]
        public string Explanation { get; set; }

        [Required]
        public string Topic { get; set; }

        [Required]
        [EnsureArraySize(1, 5, ErrorMessage = "The Alternatives array must contain between 1 and 5 elements.")]
        public string[] Subtopics { get; set; }
    }
}
