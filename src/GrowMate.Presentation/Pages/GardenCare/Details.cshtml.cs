using GrowMate.Domain.GardenTasks;
using GrowMate.Domain.Users;
using GrowMate.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Presentation.Pages.GardenCare;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ApplicationContext _context;
    private readonly UserManager<User> _userManager;

    public DetailsModel(ApplicationContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public GardenTask? Task { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        Task = await _context.GardenTasks.Include(t => t.Plant).FirstOrDefaultAsync(t => t.Id == id && t.Plant!.UserId == user.Id);
        if (Task == null) return RedirectToPage("/GardenCare/Index");

        return Page();
    }
}
