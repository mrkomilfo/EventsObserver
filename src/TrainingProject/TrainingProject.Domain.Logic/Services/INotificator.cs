using TrainingProject.Domain;

namespace TrainingProject.DomainLogic.Services
{
    public interface INotificator
    {
        public void SendMessage(string title, string body, string receiver);
        public void Notificate(EventsUsers eventUser);
    }
}
