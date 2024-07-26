using English.Net8.Api.Dtos;
using English.Net8.Api.Dtos.Question;
using English.Net8.Api.Models;
using English.Net8.Api.Repository.Interfaces;
using English.Net8.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using static English.Net8.Api.Utils.CustomAuthorize;

namespace English.Net8.Api.Controllers
{
    [Authorize]
    [Route("api/v1/question")]
    public class QuestionController : MainController
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        [HttpGet("topics")]
        [ProducesResponseType(typeof(SuccessResponseDto<IEnumerable<QuestionTopics>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<SuccessResponseDto<IEnumerable<QuestionTopics>>>> GetTopicsAsync()
        {
            var topics = await _questionRepository.GetAllTopics();
            return SuccessResponse(topics);
        }

        [HttpGet("filtered")]
        [ProducesResponseType(typeof(SuccessResponseDto<List<OutQuestionDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<List<OutQuestionDto>>>> GetQuestion([FromQuery] FilterQuestionDto filter)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            if (!ObjectId.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, out var userId))
                return Unauthorized();

            var questions = await _questionRepository.GetFilteredQuestions(userId, filter);
            return SuccessResponse(questions);
        }

        [HttpPost("answer")]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto>> AnswerQuestion(UserAnswerDto userAnswerDto)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            if (!ObjectId.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, out var userId))
                return ErrorResponse("Invalid userId");

            if (!ObjectId.TryParse(userAnswerDto.QuestionId, out var questionId))
                return ErrorResponse("Invalid questionId");

            var alreadyAnswered = await _questionRepository.CheckUserAlreadyAnsweredQuestion(userId, questionId);

            if (alreadyAnswered)
                return ErrorResponse("This question has alreaby been answered by the user");

            var userAnswer = new UserAnswer
            {
                UserId = userId,
                QuestionId = questionId,
                AnsweredAt = DateTime.UtcNow,
                HasSucceed = userAnswerDto.HasSucceed,
                QuestionDifficulty = userAnswerDto.QuestionDifficulty,
                UserAnswerId = userAnswerDto.UserAnswerId,
            };

            await _questionRepository.AddUserAnswer(userAnswer);

            return SuccessResponse();
        }


        [HttpPost("new")]
        [ClaimsAuthorize("IsAdmin", "true")]
        [ProducesResponseType(typeof(SuccessResponseDto<OutQuestionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<OutQuestionDto>>> AddQuestion(AddQuestionDto questionDto)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            var question = QuestionConverter.ToQuestion(questionDto);
            await _questionRepository.AddNewQuestionAsync(question);
            var outQuestionDto = QuestionConverter.ToOutQuestionDto(question);
            return SuccessResponse(outQuestionDto);
        }
    }
}
