using eComm.DOMAIN.Models;

namespace eComm.PERSISTENCE.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
    }
}
