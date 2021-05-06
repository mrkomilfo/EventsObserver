using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

using TrainingProject.Common;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Categories;
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
        public ActionResult<IEnumerable<CategoryFullDto>> IndexAsync()
        {
            _logger.LogMethodCalling();

            return Ok(_categoryManager.GetCategories());
        }

        [HttpGet("names")]
        public ActionResult<IEnumerable<CategoryLiteDto>> Names()
        {
            _logger.LogMethodCalling();

            return Ok(_categoryManager.GetCategoryNames());
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<CategoryFullDto>> DetailsAsync(int categoryId)
        {
            _logger.LogMethodCallingWithObject(new { categoryId });

            return Ok(await _categoryManager.GetCategoryAsync(categoryId));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [ModelStateValidation]
        public async Task<ActionResult> CreateAsync([FromBody] CategoryCreateDto categoryCreateDto)
        {
            _logger.LogMethodCallingWithObject(categoryCreateDto);
            await _categoryManager.AddCategoryAsync(categoryCreateDto);
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
