using Menulo.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Hosting;

namespace Menulo.Infrastructure.Services;

internal sealed class LocalFileStorage : IFileStorage
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/svg+xml"
    ];

    private readonly IWebHostEnvironment _environment;

    public LocalFileStorage(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(FileUploadRequest file, CancellationToken cancellationToken = default)
    {
        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Unsupported file type.");
        }

        var extension = Path.GetExtension(file.FileName);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var webRootPath = _environment.WebRootPath;
        var relativeFolder = file.Folder.Replace('\\', '/').Trim('/');
        var targetFolder = Path.Combine(webRootPath, relativeFolder);
        Directory.CreateDirectory(targetFolder);

        var absolutePath = Path.Combine(targetFolder, safeName);
        await using var output = File.Create(absolutePath);
        await file.Content.CopyToAsync(output, cancellationToken);

        return $"/{relativeFolder}/{safeName}";
    }

    public async Task<string?> CopyAsync(string? relativePath, string targetFolder, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        var sourceRelative = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var sourceAbsolute = Path.Combine(_environment.WebRootPath, sourceRelative);
        if (!File.Exists(sourceAbsolute))
        {
            return null;
        }

        var extension = Path.GetExtension(sourceAbsolute);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var relativeFolder = targetFolder.Replace('\\', '/').Trim('/');
        var destinationFolder = Path.Combine(_environment.WebRootPath, relativeFolder);
        Directory.CreateDirectory(destinationFolder);

        var destinationAbsolute = Path.Combine(destinationFolder, safeName);
        await using var source = File.OpenRead(sourceAbsolute);
        await using var destination = File.Create(destinationAbsolute);
        await source.CopyToAsync(destination, cancellationToken);

        return $"/{relativeFolder}/{safeName}";
    }

    public Task DeleteAsync(string? relativePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.CompletedTask;
        }

        var sanitizedPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var absolutePath = Path.Combine(_environment.WebRootPath, sanitizedPath);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }
}
