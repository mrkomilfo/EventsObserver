using System.Net;
using System.Net.Mail;

using TrainingProject.Common;
using TrainingProject.Domain;

namespace TrainingProject.DomainLogic.Services
{
    public class Notificator : INotificator
    {
        private readonly ILogHelper _logger;

        public Notificator(ILogHelper logger)
        {
            _logger = logger;
        }

        public void SendMessage(string title, string body, string receiver)
        {
            _logger.LogMethodCallingWithObject(new { title, body, receiver });

            var from = new MailAddress("events.observer.notificator@gmail.com", "Events Observer");
            var to = new MailAddress(receiver);

            var message = new MailMessage(from, to)
            {
                Subject = title,
                Body = body,
                IsBodyHtml = true
            };
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("events.observer.notificator@gmail.com", "EventsObserver17"),
                EnableSsl = true
            };

            smtp.Send(message);
        }

        public void NotifyAboutEvent(EventParticipant eventUser)
        {
            _logger.LogMethodCallingWithObject(eventUser);

            var title = $"[EventObserver] {eventUser.Event.Name} уже скоро";
            var body = "<h2>Напоминание о предстоящем событии</h2>" +
                $"<p>{eventUser.Participant.UserName}, напоминаем, что уже скоро состоится мероприятие {eventUser.Event.Name}, на которое Вы записались.</p>" +
                $"<p>Время: {eventUser.Event.Start:f}</p>" +
                $"<p>Место: {eventUser.Event.Place}</p>" +
                $"<p>Стоимость: {eventUser.Event.Fee}BYN</p>";
    
            SendMessage(title, body, eventUser.Participant.Email);
        }
    }
}
