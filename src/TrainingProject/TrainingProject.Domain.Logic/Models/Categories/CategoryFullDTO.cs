using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Categories
{
    public class CategoryFullDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
