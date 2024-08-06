using English.Net8.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Question
{
    public class UserAnswerDto
    {
        [Required]
        public string QuestionId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "{0} must be between 1 and 5.")]
        public int UserAnswerId { get; set; }

        [Required]
        [EnumDataType(typeof(EnglishLevel))]
        public EnglishLevel QuestionDifficulty { get; set; }

        [Required]
        public bool HasSucceed { get; set; }
    }
}
