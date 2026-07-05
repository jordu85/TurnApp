using Resend;
using TurnApp.Utils;


namespace TurnApp.Services
{
    public class EmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task Execute(EmailMessage message)
        {
            var res = await _resend.EmailSendAsync(message);
            Console.WriteLine($"Response: {res}");
        }

        public async Task SendResetPwdAsync(string dni, string callbackUrl)
        {
            var message = new EmailMessage();

            message.From = "noreply <onboarding@resend.dev>";
            message.To.Add("delivered@resend.dev");
            // message.To.Add(user.Email);
            message.Subject = "Reset Password";

            var data = new
            {
                dni = dni,
                appName = "Empanadas Don Tito",
                resetUrl = callbackUrl
            };

            var html = HandlebarsHelper.GenerateResetPwdTemplate(data);

            message.HtmlBody = html;

            await Execute(message);
        }
    }
}
