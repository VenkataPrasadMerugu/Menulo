namespace Menulo.Application.Common.Results;

public sealed class OperationResult
{
    private OperationResult(bool succeeded, string? error, int? entityId)
    {
        Succeeded = succeeded;
        Error = error;
        EntityId = entityId;
    }

    public bool Succeeded { get; }
    public string? Error { get; }
    public int? EntityId { get; }

    public static OperationResult Success(int? entityId = null) => new(true, null, entityId);

    public static OperationResult Failure(string error) => new(false, error, null);
}
