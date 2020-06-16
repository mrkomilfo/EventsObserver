using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Categories;
using TrainingProject.Common;
using TrainingProject.Web.Filters;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryManager _categoryManager;
        private readonly ILogHelper _logger;
        public CategoriesController(ICategoryManager categoryManager, ILogHelper logger)
        {
            _categoryManager = categoryManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryLiteDTO>>> Index()
        {
            _logger.LogMethodCalling();
            return Ok(await _categoryManager.GetCategories());
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<CategoryFullDTO>> Details(int categoryId)
        {
            _logger.LogMethodCallingWithObject(new { categoryId });
            return Ok(await _categoryManager.GetCategory(categoryId));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            _logger.LogMethodCallingWithObject(categoryCreateDTO);
            if (ModelState.IsValid)
            {
                await _categoryManager.AddCategory(categoryCreateDTO);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult> Update([FromBody] Category category)
        {
            _logger.LogMethodCallingWithObject(category);
            if (ModelState.IsValid)
            {
                await _categoryManager.UpdateCategory(category);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpDelete("{categoryId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            _logger.LogMethodCallingWithObject(new { categoryId });
            await _categoryManager.DeleteCategory(categoryId, false);
            return Ok();
        }
    }
}
