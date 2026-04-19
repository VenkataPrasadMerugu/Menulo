namespace Menulo.Application.Abstractions.Authentication;

public sealed record AuthResult(bool Succeeded, string? Error = null);
