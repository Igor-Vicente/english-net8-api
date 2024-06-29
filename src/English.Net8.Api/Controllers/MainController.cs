using English.Net8.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace English.Net8.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected ICollection<string> Errors = new List<string>();

        protected ActionResult<SuccessResponseDto<T>> SuccessResponse<T>(T data)
        {
            return Ok(new SuccessResponseDto<T>(data));
        }

        protected ActionResult<SuccessResponseDto> SuccessResponse()
        {
            return Ok(new SuccessResponseDto());
        }

        protected ActionResult ErrorResponse(string errorMessage)
        {
            Errors.Add(errorMessage);
            return ErrorResponse();
        }

        protected ActionResult ErrorResponse(IEnumerable<string> errors)
        {
            foreach (var error in errors)
                Errors.Add(error);
            return ErrorResponse();
        }

        protected ActionResult ErrorResponse(ModelStateDictionary modelState)
        {
            foreach (var error in modelState.Values.SelectMany(e => e.Errors))
                Errors.Add(error.ErrorMessage);
            return ErrorResponse();
        }

        private ActionResult ErrorResponse()
        {
            return BadRequest(new ErrorResponseDto(Errors));
        }
    }
}
