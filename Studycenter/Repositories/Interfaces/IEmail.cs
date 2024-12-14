namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IEmail
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
