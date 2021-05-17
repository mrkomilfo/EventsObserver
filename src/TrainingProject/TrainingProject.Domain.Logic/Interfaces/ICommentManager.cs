using System;
using System.Threading.Tasks;

using TrainingProject.DomainLogic.Models.Comments;
using TrainingProject.DomainLogic.Models.Common;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface ICommentManager
    {
        Task AddCommentAsync(CommentPostDto comment);

        Task DeleteCommentAsync(int commentId);

        Task<Page<CommentDto>> GetCommentsAsync(int eventId, int index, int pageSize);

        Task<Guid?> GetCommentAuthorIdAsync(int commentId);
    }
}