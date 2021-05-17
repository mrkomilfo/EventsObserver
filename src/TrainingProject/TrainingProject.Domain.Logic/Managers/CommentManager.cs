using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Comments;
using TrainingProject.DomainLogic.Models.Common;

namespace TrainingProject.DomainLogic.Managers
{
    public class CommentManager : ICommentManager
    {
        private readonly IAppContext _appContext;
        private readonly IMapper _mapper;

        public CommentManager(IAppContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task AddCommentAsync(CommentPostDto comment)
        {
            var newComment = _mapper.Map<Comment>(comment);

            await _appContext.Comments.AddAsync(newComment);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _appContext.Comments.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new NullReferenceException($"Comment with id={commentId} not found");
            }

            _appContext.Comments.Remove(comment);

            await _appContext.SaveChangesAsync(default);
        }

        public async Task<Page<CommentDto>> GetCommentsAsync(int eventId, int index, int pageSize)
        {
            var result = new Page<CommentDto> { CurrentPage = index, PageSize = pageSize };
            var query = _appContext.Comments.Include(c => c.Author)
                .Where(c => c.EventId == eventId).AsQueryable();

            result.TotalRecords = await query.CountAsync();

            query = query.OrderByDescending(x => x.PublicationTime).Skip(index * pageSize).Take(pageSize);

            result.Records = await _mapper.ProjectTo<CommentDto>(query).ToListAsync();

            return result;
        }

        public async Task<Guid?> GetCommentAuthorIdAsync(int commentId)
        {
            if (!await _appContext.Comments.AnyAsync(c => c.Id == commentId))
            {
                throw new NullReferenceException($"Comment with id={commentId} not found");
            }

            return (await _appContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId))?.AuthorId;
        }
    }
}