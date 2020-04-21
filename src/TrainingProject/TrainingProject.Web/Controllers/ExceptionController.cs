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
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(409, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Unhandled error: {ex.Message}");
            }
        }
    }
}
