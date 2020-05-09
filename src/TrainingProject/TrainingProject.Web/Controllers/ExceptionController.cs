using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TrainingProject.Web.Controllers
{
    public class ExceptionController : ControllerBase
    {
        public async Task<ActionResult> HandleExceptions(Func<Task<ActionResult>> action)
        {
            try
            {
                return await action.Invoke();
            }
            catch (NullReferenceException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Unhandled error: {ex.Message}" });
            }
        }
    }
}
