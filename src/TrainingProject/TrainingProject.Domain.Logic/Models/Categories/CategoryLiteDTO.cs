using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Categories
{
    public class CategoryLiteDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
