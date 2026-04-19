namespace Menulo.Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<string> SaveAsync(FileUploadRequest file, CancellationToken cancellationToken = default);
    Task<string?> CopyAsync(string? relativePath, string targetFolder, CancellationToken cancellationToken = default);
    Task DeleteAsync(string? relativePath, CancellationToken cancellationToken = default);
}
