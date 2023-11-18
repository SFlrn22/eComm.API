using eComm.APPLICATION.Contracts;
using eComm.PERSISTENCE.Contracts;

namespace eComm.APPLICATION.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
