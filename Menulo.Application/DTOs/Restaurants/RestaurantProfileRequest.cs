using System.ComponentModel.DataAnnotations;
using Menulo.Application.Abstractions.Storage;
using Menulo.Domain.Modules.Restaurants;

namespace Menulo.Application.DTOs.Restaurants;

public sealed class RestaurantProfileRequest
{
    public int? RestaurantId { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(120)]
    public string? BranchName { get; set; }

    [StringLength(240)]
    public string? Address { get; set; }

    public CurrencyCode CurrencyCode { get; set; } = CurrencyCode.INR;

    public int? ImportSourceRestaurantId { get; set; }

    public FileUploadRequest? LogoUpload { get; set; }
}
