using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Index("INDEX_CATEGORY", IsClustered = true, IsUnique = true)]
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }
    }
}
