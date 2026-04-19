namespace Menulo.Application.Abstractions.Storage;

public sealed record FileUploadRequest(
    string FileName,
    string ContentType,
    Stream Content,
    string Folder);
