using System.Net;
using System.Net.Mail;
using TrainingProject.Domain;


namespace TrainingProject.DomainLogic.Services
{
    public class Notificator : INotificator
    {
        public void SendMessage(string title, string body, string receiver)
        {
            MailAddress from = new MailAddress("events.observer.notificator@gmail.com", "Events Observer");
            MailAddress to = new MailAddress(receiver);
            MailMessage m = new MailMessage(from, to);
            m.Subject = title;
            m.Body = body;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("events.observer.notificator@gmail.com", "EventsObserver17");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }

        public void Notificate(EventsUsers eventUser)
        {
            string title = $"{eventUser.Event.Name} уже скоро";
            string body = "<h2>Напоминание о предстоящем событии</h2>" +
                $"<p>{eventUser.Participant.UserName}, напоминаем, что уже скоро состоится мероприятие {eventUser.Event.Name}, на которое Вы записались.</p>" +
                $"<p>Время: {eventUser.Event.Start.ToString("f")}</p>" +
                $"<p>Место: {eventUser.Event.Place}</p>" +
                $"<p>Взнос: {eventUser.Event.Fee}BYN</p>";
            SendMessage(title, body, eventUser.Participant.ContactEmail);
        }
    }
}
