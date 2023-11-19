using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;

namespace eComm.APPLICATION.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static UserDTO ToUserDTO(this User user)
        {
            UserDTO dto = new UserDTO()
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Username = user.Username,
                Email = user.Email,
            };
            return dto;
        }
    }
}
