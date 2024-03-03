using eComm.DOMAIN.Models;
using eComm.DOMAIN.Requests;

namespace eComm.PERSISTENCE.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username);
        Task<int> CreateUser(UserCreateRequest request);
    }
}
