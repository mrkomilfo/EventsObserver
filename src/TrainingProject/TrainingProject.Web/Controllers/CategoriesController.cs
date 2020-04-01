using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Categories;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private ICategoryManager _categoryManager;
        public CategoriesController(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<ActionResult<IEnumerable<CategoryLiteDTO>>> Index()
        {
            return Ok(await _categoryManager.GetCategories());
        }

        [HttpGet("{categoryId}")]
        [Route("Details")]
        public async Task<ActionResult<CategoryLiteDTO>> Details([FromQuery] int categoryId)
        {
            return await _categoryManager.GetCategory(categoryId)
                .ToResult("Category not found")
                .Finally(result => result.IsSuccess ? (ActionResult)Ok(result.Value) : BadRequest(result.Error));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Create")]
        public async Task<ActionResult> Create([FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var isOk = await _categoryManager.AddCategory(categoryCreateDTO);
                return isOk ? (ActionResult)Ok() : BadRequest("This category already existы");
            }
            return BadRequest("Model state is not valid");
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("Update")]
        public async Task<ActionResult> Update([FromForm] Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryManager.UpdateCategory(category);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromQuery] int categoryId)
        {
            _categoryManager.DeleteCategory(categoryId, false);
            return Ok();
        }
    }
}
