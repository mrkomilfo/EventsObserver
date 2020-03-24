using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingProject.Data.Interfaces;
using TrainingProject.Domain;

namespace TrainingProject.Data.Repositories
{
    public class UserRepository: BaseRepository, IUserRepository
    {
        public UserRepository(string connectionString, IAppContextFactory contextFactory) : base(connectionString, contextFactory) { }

        public async Task AddUser(User user)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            var user = new User() { Id = id };
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task<User> GetUser(string login)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            return await context.Users.Include(u=>u.Role).FirstOrDefaultAsync(u => u.Login == login);
        }

        /*public async Task<Page<User>> GetUsers(int index, int pageSize)
        {
            var result = new Page<User>() { CurrentPage = index, PageSize = pageSize };
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            var query = context.Users.Include(u => u.Role).AsQueryable();
            result.TotalRecords = await query.CountAsync();
            result.Records = await query.ToListAsync();
            return result;
        }*/

        public async Task<bool> IsUserExist(string login)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            return context.Users.Any(u => u.Login == login);
        }

        public async Task UpdateUser(User user)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
