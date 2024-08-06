using English.Net8.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Question
{
    public class FilterQuestionDto
    {
        [Required]
        public string Topic { get; set; }

        [Required]
        public string Subtopic { get; set; }

        [Required]
        [EnumDataType(typeof(EnglishLevel))]
        public EnglishLevel Difficulty { get; set; }

        [Required]
        public bool AlreadyAnswered { get; set; }
    }
}
