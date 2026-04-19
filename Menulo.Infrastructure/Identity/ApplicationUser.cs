using Microsoft.AspNetCore.Identity;

namespace Menulo.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
