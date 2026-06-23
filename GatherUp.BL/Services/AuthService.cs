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

    /// <summary>
    /// Admin יוצר משתמש חדש. מחזיר את הסיסמה הזמנית — פעם אחת בלבד.
    /// </summary>
    public (AppUser user, string plainPassword) CreateUser(
        string username, string role = "User")
    {
        if (userRepository.GetByUsername(username) is not null)
            throw new Core.Exceptions.BusinessRuleException($"שם המשתמש '{username}' כבר קיים.");

        var plainPassword = GeneratePassword();
        var user = new AppUser
        {
            Username     = username,
            Role         = role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword),
        };
        userRepository.Add(user);
        return (user, plainPassword);
    }

    public IEnumerable<AppUser> GetAllUsers() => userRepository.GetAll();

    public void DeleteUser(Guid id) => userRepository.Delete(id);

    private static string GeneratePassword()
    {
        const string chars = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@#$";
        var rng = new Random();
        return new string(Enumerable.Range(0, 10)
            .Select(_ => chars[rng.Next(chars.Length)]).ToArray());
    }
}
