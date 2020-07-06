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

            MailAddress from = new MailAddress("events.observer.notificator@gmail.com", "Events Observer");
            MailAddress to = new MailAddress(receiver);
            MailMessage m = new MailMessage(from, to);
            m.Subject = title;
            m.Body = body;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("events.observer.notificator@gmail.com", "EventsObserver17");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }

        public void Notificate(EventsUsers eventUser)
        {
            _logger.LogMethodCallingWithObject(eventUser);

            string title = $"[EventObserver] {eventUser.Event.Name} уже скоро";
            string body = "<h2>Напоминание о предстоящем событии</h2>" +
                $"<p>{eventUser.Participant.UserName}, напоминаем, что уже скоро состоится мероприятие {eventUser.Event.Name}, на которое Вы записались.</p>" +
                $"<p>Время: {eventUser.Event.Start:f}</p>" +
                $"<p>Место: {eventUser.Event.Place}</p>" +
                $"<p>Взнос: {eventUser.Event.Fee}BYN</p>";
            SendMessage(title, body, eventUser.Participant.ContactEmail);
        }
    }
}
