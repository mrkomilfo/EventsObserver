using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using TrainingProject.Web.Jobs;

namespace TrainingProject.Web.Services
{
    public class Sheduler : Registry
    {
        public Sheduler(IServiceProvider sp)
        {
            Schedule(sp.CreateScope().ServiceProvider.GetRequiredService<SendMessageJob>())
                .ToRunNow().AndEvery(1).Hours();
        }
    }
}
