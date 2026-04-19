namespace Menulo.Application.Abstractions.Persistence;

public sealed class OwnerIdentityDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
