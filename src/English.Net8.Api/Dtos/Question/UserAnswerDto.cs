using English.Net8.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Question
{
    public class UserAnswerDto
    {
        [Required]
        public string QuestionId { get; set; }

        [Required]
        public int UserAnswerId { get; set; }

        [Required]
        [EnumDataType(typeof(Difficulty))]
        public Difficulty QuestionDifficulty { get; set; }

        [Required]
        public bool HasSucceed { get; set; }
    }
}
