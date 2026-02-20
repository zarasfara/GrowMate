using GrowMate.Application.Services;
using GrowMate.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrowMate.Presentation.Pages.GardenBeds;

public class DetailsModel : PageModel
{
    private readonly GardenBedService _gardenBedService;
    private readonly UserManager<User> _userManager;

    public DetailsModel(GardenBedService gardenBedService, UserManager<User> userManager)
    {
        _gardenBedService = gardenBedService;
        _userManager = userManager;
    }

    public GardenBedDto? GardenBed { get; set; }
    public string? ErrorMessage { get; set; }

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

