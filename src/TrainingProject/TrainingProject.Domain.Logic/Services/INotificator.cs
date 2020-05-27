using TrainingProject.Domain;

namespace TrainingProject.DomainLogic.Services
{
    public interface INotificator
    {
        public void Notificate(EventsUsers eventUser);
    }
}
