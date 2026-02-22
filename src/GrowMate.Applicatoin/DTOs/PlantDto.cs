using GrowMate.Domain.Plants;

namespace GrowMate.Applicatoin.DTOs;

public class PlantDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Variety { get; set; }
    
    public string? Description { get; set; }

    public string? ImagePath { get; set; }
    
    public DateTime PlantingDate { get; set; }
    
    public PlantType Type { get; set; }
    
    public int Quantity { get; set; }
    
    public bool IsUnique { get; set; }
    
    public int? PlantTemplateId { get; set; }
    
    public string? TemplateName { get; set; }
    
    public int GardenBedId { get; set; }
    
    public string GardenBedName { get; set; } = string.Empty;
    
    public int DaysFromPlanting => (DateTime.Now - PlantingDate).Days;
}


