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
        [ProducesResponseType(typeof(SuccessResponseDto<OutQuestionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<OutQuestionDto>>> GetQuestion([FromQuery] FilterQuestionDto filter, [FromHeader] IEnumerable<string> seenQuestionIds)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            if (!ObjectId.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value, out var userId))
                return ErrorResponse("Invalid userId");

            var seenQuestionObjectIds = new List<ObjectId>();
            foreach (var questionIdString in seenQuestionIds)
            {
                if (!ObjectId.TryParse(questionIdString, out var questionId))
                    return ErrorResponse($"{questionIdString} could not be converted to a questionId");
                seenQuestionObjectIds.Add(questionId);
            }

            var question = await _questionRepository.GetFilteredQuestion(userId, filter, seenQuestionObjectIds);
            return SuccessResponse(question);
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

            var alreadyAnswered = await _questionRepository.CheckUserAlreadyAnswerTheQuestion(userId, questionId);

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
