using SCMS_back_end.Repositories.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace SCMS_back_end.Repositories.Services
{
    public class EmailService: IEmail
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["SendGrid:ApiKey"];
            _fromEmail = _configuration["SendGrid:FromEmail"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string emailDescription)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, "Learnify");
            var to = new EmailAddress(toEmail);

            // Create the email body with the confirmation link
            //var htmlContent = $@"
            //    <p>Hello,</p>
            //    <p>Please confirm your email address by clicking the following link:</p>
            //    <p><a href='{confirmationLink}'>Confirm Email</a></p>
            //    <p>Thank you!</p>";
            var htmlContent = emailDescription;

            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

            var response = await client.SendEmailAsync(msg);

            //Handle response
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // Log or handle error
                var responseBody = await response.Body.ReadAsStringAsync();
                // Log or process responseBody as needed
                throw new Exception($"Email sending failed. Status code: {response.StatusCode}, Response: {responseBody}");
            }
        }
    }
}
