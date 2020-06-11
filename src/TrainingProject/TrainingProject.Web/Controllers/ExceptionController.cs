using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
            catch (KeyNotFoundException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status404NotFound, new { ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status409Conflict, new { ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status401Unauthorized, new { ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Unhandled error: {ex.Message}" });
            }
        }
    }
}
