namespace eComm.APPLICATION.Contracts
{
    public interface ILogRepository
    {
        Task LogSuccess<Req, Resp>(Req request, Resp response, string username, string sessionIdentifier, string endpoint);
        Task LogException<Req>(Req request, Exception ex, string username, string sessionIdentifier, string endpoint);
    }
}
