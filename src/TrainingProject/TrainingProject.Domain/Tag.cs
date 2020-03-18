using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class Tag
    {
        public int TagId { get; set; }
        
        [Index("INDEX_TAG", IsClustered = true, IsUnique = true)]
        public string Name { get; set; }
    }
}
