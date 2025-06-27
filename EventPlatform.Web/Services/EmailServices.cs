using System.Net;
using System.Net.Mail;

namespace EventPlatform.Services
{
    public interface IEmailService
    {
        Task SendEmailConfirmation(string email, string token);
        Task SendPasswordReset(string email, string token);
        Task SendEventApprovalNotification(string email, string eventTitle, bool isApproved, string comments = null);
        Task SendTicketPurchaseConfirmation(string email, string eventTitle, string ticketNumber);
        Task SendEventReminder(string email, string eventTitle, DateTime eventDate);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["Port"]),
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailConfirmation(string email, string token)
        {
            var subject = "Подтверждение регистрации";
            var body = $@"
                <h1>Подтвердите ваш email</h1>
                <p>Для завершения регистрации перейдите по <a href='https://yourapp.com/confirm-email?token={token}'>ссылке</a>.</p>
                <p>Ссылка действительна в течение 24 часов.</p>
            ";
            await SendEmail(email, subject, body);
        }

        public async Task SendPasswordReset(string email, string token)
        {
            var subject = "Сброс пароля";
            var body = $@"
                <h1>Сброс пароля</h1>
                <p>Для сброса пароля перейдите по <a href='https://yourapp.com/reset-password?token={token}'>ссылке</a>.</p>
                <p>Ссылка действительна в течение 1 часа.</p>
            ";
            await SendEmail(email, subject, body);
        }

        public async Task SendEventApprovalNotification(string email, string eventTitle, bool isApproved, string comments = null)
        {
            var subject = isApproved ? "Ваше мероприятие одобрено" : "Ваше мероприятие отклонено";
            var body = isApproved
                ? $@"
                    <h1>Мероприятие &laquo;{eventTitle}&raquo; одобрено</h1>
                    <p>Ваше мероприятие было одобрено и теперь доступно для просмотра и покупки билетов.</p>
                "
                : $@"
                    <h1>Мероприятие &laquo;{eventTitle}&raquo; отклонено</h1>
                    <p>Ваше мероприятие было отклонено модератором.</p>
                    {(string.IsNullOrEmpty(comments) ? "" : $"<p>Комментарий модератора: {comments}</p>")}
                    <p>Вы можете исправить замечания и отправить мероприятие на повторную модерацию.</p>
                ";
            await SendEmail(email, subject, body);
        }

        public async Task SendTicketPurchaseConfirmation(string email, string eventTitle, string ticketNumber)
        {
            var subject = "Подтверждение покупки билета";
            var body = $@"
                <h1>Спасибо за покупку!</h1>
                <p>Вы успешно приобрели билет на мероприятие &laquo;{eventTitle}&raquo;.</p>
                <p>Номер вашего билета: <strong>{ticketNumber}</strong></p>
                <p>Билет доступен в вашем личном кабинете.</p>
            ";
            await SendEmail(email, subject, body);
        }

        public async Task SendEventReminder(string email, string eventTitle, DateTime eventDate)
        {
            var subject = "Напоминание о мероприятии";
            var body = $@"
                <h1>Напоминание о мероприятии</h1>
                <p>Завтра состоится мероприятие &laquo;{eventTitle}&raquo;, на которое вы приобрели билет.</p>
                <p>Дата и время: {eventDate:dd.MM.yyyy HH:mm}</p>
                <p>Не забудьте взять с собой билет (QR-код) из личного кабинета.</p>
            ";
            await SendEmail(email, subject, body);
        }
    }
}