using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Comments;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.Web.Filters;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentManager _commentManager;

        public CommentsController(ICommentManager commentManager)
        {
            _commentManager = commentManager;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Create([FromBody] CommentPostDto comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model state is not valid");
            }

            await _commentManager.AddCommentAsync(comment);

            return Ok();
        }

        [HttpDelete("{commentId:int}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Delete([FromRoute] int commentId)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var authorId = await _commentManager.GetCommentAuthorIdAsync(commentId);

            if (role != "Admin" && Guid.Parse(userId) != authorId)
            {
                return Forbid("Access denied");
            }

            await _commentManager.DeleteCommentAsync(commentId);

            return Ok();
        }

        [HttpGet("{eventId:int}")]
        public async Task<ActionResult<Page<CommentDto>>> Index(
            [FromRoute] int eventId, [FromQuery] int index=0, [FromQuery] int pageSize=8)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;

            var comments = await _commentManager.GetCommentsAsync(eventId, index, pageSize);

            foreach (var comment in comments.Records)
            {
                comment.CanDelete = role == "Admin" || userId == comment.Author.Id;
            }

            return Ok(comments);
        }
    }
}