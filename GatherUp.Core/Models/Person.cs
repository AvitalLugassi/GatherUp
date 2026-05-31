using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;

public abstract class Person : IIdentifiable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
}
