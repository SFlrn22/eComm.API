namespace eComm.APPLICATION.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string body, string destination);
    }
}
