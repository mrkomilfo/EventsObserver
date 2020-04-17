using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<IEnumerable<CategoryLiteDTO>>> Index()
        {
            return Ok(await _categoryManager.GetCategories());
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<CategoryFullDTO>> Details(int categoryId)
        {
            return Ok(await _categoryManager.GetCategory(categoryId));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
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
            if (ModelState.IsValid)
            {
                await _categoryManager.UpdateCategory(category);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int categoryId)
        {
            _categoryManager.DeleteCategory(categoryId, false);
            return Ok();
        }
    }
}
