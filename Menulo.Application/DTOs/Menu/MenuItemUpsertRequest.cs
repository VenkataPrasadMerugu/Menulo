using System.ComponentModel.DataAnnotations;
using Menulo.Application.Abstractions.Storage;
using Menulo.Domain.Modules.Menu;

namespace Menulo.Application.DTOs.Menu;

public sealed class MenuItemUpsertRequest
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal Price { get; set; }

    [Range(1, 50)]
    public int Serves { get; set; } = 1;

    public MenuCategory Category { get; set; } = MenuCategory.MainCourse;

    public FoodType FoodType { get; set; } = FoodType.Veg;

    public IReadOnlyList<FileUploadRequest> ImageUploads { get; set; } = [];
}
