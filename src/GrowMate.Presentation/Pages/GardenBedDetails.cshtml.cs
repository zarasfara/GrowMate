using GrowMate.Domain.Users;
using GrowMate.Presentation.Models;
using GrowMate.Presentation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrowMate.Presentation.Pages;

public class GardenBedDetailsModel : PageModel
{
    private readonly GardenBedService _gardenBedService;
    private readonly UserManager<User> _userManager;

    public GardenBedDto? GardenBed { get; set; }
    public string? ErrorMessage { get; set; }

    public GardenBedDetailsModel(GardenBedService gardenBedService, UserManager<User> userManager)
    {
        _gardenBedService = gardenBedService;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        GardenBed = await _gardenBedService.GetGardenBedByIdAsync(id, user.Id);
        if (GardenBed == null)
        {
            ErrorMessage = "Грядка не найдена.";
            return NotFound();
        }

        return Page();
    }
}

