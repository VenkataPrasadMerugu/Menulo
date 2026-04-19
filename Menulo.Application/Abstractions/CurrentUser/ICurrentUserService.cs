namespace Menulo.Application.Abstractions.CurrentUser;

public interface ICurrentUserService
{
    int? UserId { get; }
    bool IsAuthenticated { get; }
}
