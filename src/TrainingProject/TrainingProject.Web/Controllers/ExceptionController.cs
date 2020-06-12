using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrainingProject.Common;

namespace TrainingProject.Web.Controllers
{
    public class ExceptionController : ControllerBase
    {
        private readonly ILogHelper _logger;

        public ExceptionController(ILogHelper logger) 
        {
            _logger = logger;
        }

        public async Task<ActionResult> HandleExceptions(Func<Task<ActionResult>> action)
        {
            try
            {
                return await action.Invoke();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex);
                return StatusCode(StatusCodes.Status404NotFound, new { ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex);
                return StatusCode(StatusCodes.Status409Conflict, new { ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex);
                return StatusCode(StatusCodes.Status401Unauthorized, new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Unhandled error: {ex.Message}" });
            }
        }
    }
}
