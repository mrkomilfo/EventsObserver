namespace TrainingProject.Data.Interfaces
{
    public interface IAppContextFactory
    {
        AppContext CreateDbContext(string connectionString);
    }
}
