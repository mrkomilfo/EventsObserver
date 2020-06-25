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
        public async Task<ActionResult<IEnumerable<CategoryLiteDTO>>> IndexAsync()
        {
            _logger.LogMethodCalling();
            return Ok(await _categoryManager.GetCategoriesAsync());
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<CategoryFullDTO>> DetailsAsync(int categoryId)
        {
            _logger.LogMethodCallingWithObject(new { categoryId });
            return Ok(await _categoryManager.GetCategoryAsync(categoryId));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [ModelStateValidation]
        public async Task<ActionResult> CreateAsync([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            _logger.LogMethodCallingWithObject(categoryCreateDTO);
            await _categoryManager.AddCategoryAsync(categoryCreateDTO);
            return Ok();
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [ModelStateValidation]
        public async Task<ActionResult> UpdateAsync([FromBody] Category category)
        {
            _logger.LogMethodCallingWithObject(category);
            await _categoryManager.UpdateCategoryAsync(category);
            return Ok();
        }

        [HttpDelete("{categoryId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync(int categoryId)
        {
            _logger.LogMethodCallingWithObject(new { categoryId });
            await _categoryManager.DeleteCategoryAsync(categoryId, false);
            return Ok();
        }
    }
}
