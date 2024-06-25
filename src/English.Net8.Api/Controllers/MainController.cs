using English.Net8.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace English.Net8.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected ICollection<string> Errors { get; set; } = new List<string>();

        protected IActionResult CustomResponse(object result = null)
        {
            if (!Errors.Any())
            {
                return Ok(new ResponseDto
                {
                    Success = true,
                    Data = result
                });
            }
            else
            {
                return BadRequest(new ResponseDto
                {
                    Success = false,
                    Errors = Errors
                });
            }
        }

        protected IActionResult CustomResponse(ModelStateDictionary modelState)
        {
            foreach (var error in modelState.Values.SelectMany(x => x.Errors))
                NotifierError(error.ErrorMessage);
            return CustomResponse();
        }

        protected void NotifierError(string error) => Errors.Add(error);
        protected void NotifierError(IEnumerable<string> errors)
        {
            foreach (var error in errors)
                Errors.Add(error);
        }
    }
}
