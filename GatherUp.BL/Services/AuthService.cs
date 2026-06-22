using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;

namespace GatherUp.BL.Services;

public class AuthService(IUserRepository userRepository)
{
    public AppUser? ValidateUser(string username, string password)
    {
        var user = userRepository.GetByUsername(username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        return user;
    }
}
