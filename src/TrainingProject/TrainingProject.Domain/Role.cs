using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class Role
    {
        public byte Id { get; set; }
        [Index("INDEX_ROLE", IsClustered = true, IsUnique = true)]
        public string Name { get; set; }
    }
}
