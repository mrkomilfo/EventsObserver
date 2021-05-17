using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Models.Comments
{
    public class CommentDto
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string PublicationTime { get; set; }

        public bool CanDelete { get; set; }

        public CommentAuthorDto Author { get; set; }
    }
}