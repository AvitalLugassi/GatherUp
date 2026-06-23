using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;
public class AppUser : IIdentifiable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; } // "Admin" | "User"
    public string Email { get; set; } = string.Empty;
}
