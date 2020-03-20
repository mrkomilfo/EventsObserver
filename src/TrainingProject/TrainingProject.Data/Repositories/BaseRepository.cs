using TrainingProject.Data.Interfaces;

namespace TrainingProject.Data.Repositories
{
    public abstract class BaseRepository
    {
        protected string ConnectionString { get; }
        protected IAppContextFactory ContextFactory { get; }

        protected BaseRepository(string connectionString, IAppContextFactory contextFactory)
        {
            ConnectionString = connectionString;
            ContextFactory = contextFactory;
        }
    }
}
