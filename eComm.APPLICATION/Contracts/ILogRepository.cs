namespace eComm.APPLICATION.Contracts
{
    public interface ILogRepository
    {
        Task LogSuccess<Req, Resp>(Req request, Resp response, string username, string sessionIdentifier);
        Task LogException<Req>(Req request, Exception ex, string username, string sessionIdentifier);
    }
}
