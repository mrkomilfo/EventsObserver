using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class Tag
    {
        public int Id { get; set; }
        [Index("INDEX_TAG", IsClustered = true, IsUnique = true)]
        public string Name { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
