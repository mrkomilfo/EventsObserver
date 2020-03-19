using Microsoft.EntityFrameworkCore;
using TrainingProject.Data.Interfaces;

namespace TrainingProject.Data
{
    public class AppContextFactory: IAppContextFactory
    {
        public AppContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppContext(optionsBuilder.Options);
        }
    }
}
