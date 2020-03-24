using Microsoft.Extensions.DependencyInjection;
using TrainingProject.Data;

namespace TrainingProject.DomainLogic
{
    public static class DomainLogicExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddDataServices();
            //configure your Domain Logic Layer services here
            return services;
        }
    }
}