using FluentScheduler;
using TrainingProject.DomainLogic.Interfaces;

namespace TrainingProject.Web.Jobs
{
    public class SendMessageJob : IJob
    {
        private IEventManager _eventManager;

        public SendMessageJob(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public void Execute()
        {
            _eventManager.Notificate();
        }
    }
}
