using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class Category
    {
        public int Id { get; set; }
        [Index("INDEX_CATEGORY", IsClustered = true, IsUnique = true)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
