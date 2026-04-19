using System.ComponentModel.DataAnnotations;
using Menulo.Application.Abstractions.Storage;

namespace Menulo.Application.DTOs.Restaurants;

public sealed class BrandingSettingsRequest
{
    public string? PaletteKey { get; set; }

    [RegularExpression("^#(?:[0-9a-fA-F]{6})$")]
    public string? PrimaryColor { get; set; }

    [RegularExpression("^#(?:[0-9a-fA-F]{6})$")]
    public string? SecondaryColor { get; set; }

    public FileUploadRequest? LogoUpload { get; set; }
}
