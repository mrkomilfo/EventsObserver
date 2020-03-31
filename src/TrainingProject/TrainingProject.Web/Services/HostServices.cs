using Microsoft.AspNetCore.Hosting;
using TrainingProject.Web.Interfaces;

namespace TrainingProject.Web.Services
{
    public class HostServices : IHostServices
    {
        private readonly IWebHostEnvironment _environment;
        public HostServices(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public string GetHostPath()
        {
            return _environment.WebRootPath;
        }
    }
}
