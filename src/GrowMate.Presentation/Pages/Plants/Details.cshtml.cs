using GrowMate.Applicatoin.DTOs;
using GrowMate.Applicatoin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace GrowMate.Presentation.Pages.Plants;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly PlantService _plantService;
    private readonly ILogger<DetailsModel> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DetailsModel(PlantService plantService, ILogger<DetailsModel> logger, IWebHostEnvironment webHostEnvironment)
    {
        _plantService = plantService;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    public PlantDto Plant { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in claims");
            return RedirectToPage("/Account/Login");
        }

        var plant = await _plantService.GetPlantByIdAsync(id, userId);
        if (plant == null)
        {
            _logger.LogWarning("Plant with ID {PlantId} not found", id);
            return NotFound();
        }

        Plant = plant;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in claims");
            return RedirectToPage("/Account/Login");
        }

        var plant = await _plantService.GetPlantByIdAsync(id, userId);
        var imagePath = plant?.ImagePath;

        var result = await _plantService.DeletePlantAsync(id, userId);
        if (result)
        {
            DeleteImageIfExists(imagePath);
            TempData["SuccessMessage"] = "Посадка успешно удалена!";
        }
        else
        {
            TempData["ErrorMessage"] = "Не удалось удалить посадку.";
        }

        return RedirectToPage("/Plants/Index");
    }

    private void DeleteImageIfExists(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        var relativePath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
        if (!System.IO.File.Exists(fullPath))
        {
            return;
        }

        try
        {
            System.IO.File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete image file {ImagePath}", fullPath);
        }
    }
}


