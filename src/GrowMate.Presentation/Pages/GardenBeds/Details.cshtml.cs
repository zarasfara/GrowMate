﻿using GrowMate.Applicatoin.DTOs;
using GrowMate.Applicatoin.Services;
using GrowMate.Application.Services;
using GrowMate.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrowMate.Presentation.Pages.GardenBeds;

public class DetailsModel : PageModel
{
    private readonly GardenBedService _gardenBedService;
    private readonly PlantService _plantService;
    private readonly UserManager<User> _userManager;

    public DetailsModel(GardenBedService gardenBedService, PlantService plantService, UserManager<User> userManager)
    {
        _gardenBedService = gardenBedService;
        _plantService = plantService;
        _userManager = userManager;
    }

    public GardenBedDto? GardenBed { get; set; }
    public List<PlantDto> Plants { get; set; } = [];
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

        Plants = await _plantService.GetPlantsByGardenBedAsync(id, user.Id);

        return Page();
    }
}

