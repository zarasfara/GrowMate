using GrowMate.Applicatoin.DTOs;
using GrowMate.Domain.Plants;
using GrowMate.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GrowMate.Applicatoin.Services;

public class PlantService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<PlantService> _logger;

    public PlantService(ApplicationContext context, ILogger<PlantService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PlantDto>> GetAllPlantsAsync(string userId)
    {
        return await _context.Plants
            .Where(p => p.UserId == userId)
            .Include(p => p.GardenBed)
            .Include(p => p.Template)
            .Select(p => new PlantDto
            {
                Id = p.Id,
                Name = p.Name,
                Variety = p.Variety,
                Description = p.Description,
                ImagePath = p.ImagePath,
                PlantingDate = p.PlantingDate,
                Type = p.Type,
                Quantity = p.Quantity,
                IsUnique = p.IsUnique,
                PlantTemplateId = p.PlantTemplateId,
                TemplateName = p.Template != null ? p.Template.Name : null,
                GardenBedId = p.GardenBedId,
                GardenBedName = p.GardenBed != null ? p.GardenBed.Name : string.Empty
            })
            .ToListAsync();
    }

    public async Task<List<PlantDto>> GetPlantsByGardenBedAsync(int gardenBedId, string userId)
    {
        return await _context.Plants
            .Where(p => p.GardenBedId == gardenBedId && p.UserId == userId)
            .Include(p => p.GardenBed)
            .Include(p => p.Template)
            .Select(p => new PlantDto
            {
                Id = p.Id,
                Name = p.Name,
                Variety = p.Variety,
                Description = p.Description,
                ImagePath = p.ImagePath,
                PlantingDate = p.PlantingDate,
                Type = p.Type,
                Quantity = p.Quantity,
                IsUnique = p.IsUnique,
                PlantTemplateId = p.PlantTemplateId,
                TemplateName = p.Template != null ? p.Template.Name : null,
                GardenBedId = p.GardenBedId,
                GardenBedName = p.GardenBed != null ? p.GardenBed.Name : string.Empty
            })
            .ToListAsync();
    }

    public async Task<PlantDto?> GetPlantByIdAsync(int id, string userId)
    {
        return await _context.Plants
            .Where(p => p.Id == id && p.UserId == userId)
            .Include(p => p.GardenBed)
            .Include(p => p.Template)
            .Select(p => new PlantDto
            {
                Id = p.Id,
                Name = p.Name,
                Variety = p.Variety,
                Description = p.Description,
                ImagePath = p.ImagePath,
                PlantingDate = p.PlantingDate,
                Type = p.Type,
                Quantity = p.Quantity,
                IsUnique = p.IsUnique,
                PlantTemplateId = p.PlantTemplateId,
                TemplateName = p.Template != null ? p.Template.Name : null,
                GardenBedId = p.GardenBedId,
                GardenBedName = p.GardenBed != null ? p.GardenBed.Name : string.Empty
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreatePlantAsync(CreatePlantDto dto, string userId)
    {
        _logger.LogInformation("Creating new plant: {PlantName} for user {UserId}", dto.Name, userId);

        var plant = new Plant
        {
            Name = dto.Name,
            Variety = dto.Variety,
            Description = dto.Description,
            ImagePath = dto.ImagePath,
            PlantingDate = dto.PlantingDate,
            Type = dto.Type,
            Quantity = dto.Quantity,
            IsUnique = dto.IsUnique,
            PlantTemplateId = dto.PlantTemplateId,
            GardenBedId = dto.GardenBedId,
            UserId = userId
        };

        _context.Plants.Add(plant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Plant created successfully with ID: {PlantId}", plant.Id);
        return plant.Id;
    }

    public async Task<bool> UpdatePlantAsync(UpdatePlantDto dto, string userId)
    {
        _logger.LogInformation("Updating plant with ID: {PlantId}", dto.Id);

        var plant = await _context.Plants
            .FirstOrDefaultAsync(p => p.Id == dto.Id && p.UserId == userId);

        if (plant == null)
        {
            _logger.LogWarning("Plant with ID {PlantId} not found for user {UserId}", dto.Id, userId);
            return false;
        }

        plant.Name = dto.Name;
        plant.Variety = dto.Variety;
        plant.Description = dto.Description;
        plant.ImagePath = dto.ImagePath;
        plant.PlantingDate = dto.PlantingDate;
        plant.Type = dto.Type;
        plant.Quantity = dto.Quantity;
        plant.IsUnique = dto.IsUnique;
        plant.PlantTemplateId = dto.PlantTemplateId;
        plant.GardenBedId = dto.GardenBedId;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Plant updated successfully: {PlantId}", dto.Id);
        return true;
    }

    public async Task<bool> DeletePlantAsync(int id, string userId)
    {
        _logger.LogInformation("Deleting plant with ID: {PlantId}", id);

        var plant = await _context.Plants
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (plant == null)
        {
            _logger.LogWarning("Plant with ID {PlantId} not found for user {UserId}", id, userId);
            return false;
        }

        _context.Plants.Remove(plant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Plant deleted successfully: {PlantId}", id);
        return true;
    }
}



