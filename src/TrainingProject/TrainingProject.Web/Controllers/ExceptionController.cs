using System;
using System.Threading.Tasks;
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
                return BadRequest(new { ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(409, new { ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Unhandled error: {ex.Message}" });
            }
        }
    }
}
