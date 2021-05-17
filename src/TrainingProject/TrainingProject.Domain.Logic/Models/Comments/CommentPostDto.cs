using System.ComponentModel.DataAnnotations;

using TrainingProject.DomainLogic.Annotations;

namespace TrainingProject.DomainLogic.Models.Comments
{
    public class CommentPostDto
    {
        [Required]
        public int EventId { get; set; }

        [ValidGuid]
        [Required]
        public string AuthorId { get; set; }

        [Required]
        public string Message { get; set; }
    }
}