using GrowMate.Domain.GardenBeds;
using GrowMate.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GrowMate.Application.Services;

/// <summary>
///     DTO для отображения грядки
/// </summary>
public class GardenBedDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public GardenBedType Type { get; set; }
    public int PlantCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
///     DTO для создания грядки
/// </summary>
public class CreateGardenBedDto
{
    public string? Name { get; set; }
    public GardenBedType Type { get; set; } = GardenBedType.OpenGround;
}

/// <summary>
///     DTO для обновления грядки
/// </summary>
public class UpdateGardenBedDto
{
    public string? Name { get; set; }
    public GardenBedType Type { get; set; }
}

/// <summary>
/// Сервис для работы с грядками
/// </summary>
public class GardenBedService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<GardenBedService> _logger;

    public GardenBedService(ApplicationContext context, ILogger<GardenBedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить все грядки пользователя
    /// </summary>
    public async Task<List<GardenBedDto>> GetUserGardenBedsAsync(string userId)
    {
        return await _context.GardenBeds
            .AsNoTracking()
            .Where(gb => gb.UserId == userId)
            .Include(gb => gb.Plants)
            .Select(gb => new GardenBedDto
            {
                Id = gb.Id,
                Name = gb.Name ?? string.Empty,
                Type = gb.Type,
                PlantCount = gb.Plants.Count,
                CreatedAt = DateTime.UtcNow
            })
            .OrderByDescending(gb => gb.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    ///     Получить грядку по ID
    /// </summary>
    public async Task<GardenBedDto?> GetGardenBedByIdAsync(int id, string userId)
    {
        _logger.LogInformation($"GetGardenBedByIdAsync: id={id}, userId={userId}");
        
        var gardenBed = await _context.GardenBeds
            .AsNoTracking()
            .Include(gb => gb.Plants)
            .FirstOrDefaultAsync(gb => gb.Id == id && gb.UserId == userId);

        if (gardenBed == null)
        {
            _logger.LogWarning($"GardenBed not found: id={id}, userId={userId}");
            return null;
        }

        _logger.LogInformation($"GardenBed found: {gardenBed.Name}");

        return new GardenBedDto
        {
            Id = gardenBed.Id,
            Name = gardenBed.Name ?? string.Empty,
            Type = gardenBed.Type,
            PlantCount = gardenBed.Plants.Count,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    ///     Создать новую грядку
    /// </summary>
    public async Task<GardenBedDto> CreateGardenBedAsync(CreateGardenBedDto dto, string userId)
    {
        var gardenBed = new GardenBed
        {
            Name = dto.Name ?? string.Empty,
            Type = dto.Type,
            UserId = userId
        };

        _context.GardenBeds.Add(gardenBed);
        await _context.SaveChangesAsync();

        return new GardenBedDto
        {
            Id = gardenBed.Id,
            Name = gardenBed.Name,
            Type = gardenBed.Type,
            PlantCount = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    ///     Обновить грядку
    /// </summary>
    public async Task<bool> UpdateGardenBedAsync(int id, UpdateGardenBedDto dto, string userId)
    {
        var gardenBed = await _context.GardenBeds
            .FirstOrDefaultAsync(gb => gb.Id == id && gb.UserId == userId);

        if (gardenBed == null)
            return false;

        gardenBed.Name = dto.Name ?? string.Empty;
        gardenBed.Type = dto.Type;

        _context.GardenBeds.Update(gardenBed);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     Удалить грядку
    /// </summary>
    public async Task<bool> DeleteGardenBedAsync(int id, string userId)
    {
        var gardenBed = await _context.GardenBeds
            .FirstOrDefaultAsync(gb => gb.Id == id && gb.UserId == userId);

        if (gardenBed == null)
            return false;

        _context.GardenBeds.Remove(gardenBed);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     Проверить, принадлежит ли грядка пользователю
    /// </summary>
    public async Task<bool> GardenBedBelongsToUserAsync(int id, string userId)
    {
        return await _context.GardenBeds
            .AnyAsync(gb => gb.Id == id && gb.UserId == userId);
    }
}




