using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;

public abstract class Person : IIdentifiable
{
    public Guid Id { get; set; } = Guid.NewGuid();//was init
    public required string Name { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
}
