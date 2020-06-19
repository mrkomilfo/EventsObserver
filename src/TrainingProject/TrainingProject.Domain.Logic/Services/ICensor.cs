using System.Threading.Tasks;

namespace TrainingProject.DomainLogic.Services
{
    public interface ICensor
    {
        public Task<string> HandleMessageAsync(string message);
    }
}
