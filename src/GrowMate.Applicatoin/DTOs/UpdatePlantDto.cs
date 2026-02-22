using GrowMate.Domain.Plants;
using System.ComponentModel.DataAnnotations;

namespace GrowMate.Applicatoin.DTOs;

public class UpdatePlantDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Название растения обязательно")]
    [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
    public required string Name { get; set; }
    
    [StringLength(50, ErrorMessage = "Сорт не должен превышать 50 символов")]
    public string? Variety { get; set; }
    
    public string? Description { get; set; }

    public string? ImagePath { get; set; }
    
    [Required(ErrorMessage = "Дата посадки обязательна")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [CustomValidation(typeof(UpdatePlantDto), nameof(ValidatePlantingDate))]
    public DateTime PlantingDate { get; set; }
    
    [Required(ErrorMessage = "Тип растения обязателен")]
    public PlantType Type { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
    public int Quantity { get; set; } = 1;
    
    public bool IsUnique { get; set; }
    
    public int? PlantTemplateId { get; set; }
    
    [Required(ErrorMessage = "Грядка обязательна")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите грядку")]
    public int GardenBedId { get; set; }

    public static ValidationResult? ValidatePlantingDate(DateTime plantingDate, ValidationContext context)
    {
        if (plantingDate.Date > DateTime.Today)
        {
            return new ValidationResult("Дата посадки не может быть в будущем");
        }
        
        return ValidationResult.Success;
    }
}


