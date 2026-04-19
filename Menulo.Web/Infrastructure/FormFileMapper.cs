using Menulo.Application.Abstractions.Storage;

namespace Menulo.Web.Infrastructure;

public static class FormFileMapper
{
    public static async Task<FileUploadRequest?> ToUploadAsync(IFormFile? formFile, string folder, CancellationToken cancellationToken)
    {
        if (formFile is null || formFile.Length == 0)
        {
            return null;
        }

        var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return new FileUploadRequest(formFile.FileName, formFile.ContentType, memoryStream, folder);
    }

    public static async Task<IReadOnlyList<FileUploadRequest>> ToUploadsAsync(IEnumerable<IFormFile>? formFiles, string folder, CancellationToken cancellationToken)
    {
        if (formFiles is null)
        {
            return [];
        }

        var uploads = new List<FileUploadRequest>();
        foreach (var file in formFiles.Where(x => x is not null && x.Length > 0))
        {
            var upload = await ToUploadAsync(file, folder, cancellationToken);
            if (upload is not null)
            {
                uploads.Add(upload);
            }
        }

        return uploads;
    }
}
