using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrowMate.Domain.GardenBeds;
using GrowMate.Domain.GardenTasks;
using GrowMate.Domain.Plants;
using GrowMate.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GrowMate.Presentation.Pages
{
    public class StatisticsModel : PageModel
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<GrowMate.Domain.Users.User> _userManager;

        public int TotalGardenBeds { get; set; }
        public int TotalPlants { get; set; }
        public int PendingTasks { get; set; }
        public int CompletedTasksToday { get; set; }

        public List<KeyValuePair<string,int>> PlantsByType { get; set; } = new();
        public List<KeyValuePair<string,int>> TasksByType { get; set; } = new();

        public class TopPlantDto { public Plant Plant { get; set; } = null!; public int TaskCount { get; set; } }
        public List<TopPlantDto> TopPlants { get; set; } = new();

        public StatisticsModel(ApplicationContext context, UserManager<GrowMate.Domain.Users.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            TotalGardenBeds = await _context.GardenBeds.CountAsync(b => b.UserId == user.Id);
            TotalPlants = await _context.Plants.CountAsync(p => p.UserId == user.Id);
            PendingTasks = await _context.GardenTasks.CountAsync(t => t.Plant != null && t.Plant.UserId == user.Id && !t.IsCompleted);
            CompletedTasksToday = await _context.GardenTasks.CountAsync(t => t.Plant != null && t.Plant.UserId == user.Id && t.IsCompleted && t.DueDate.Date == System.DateTime.Today);

            PlantsByType = await _context.Plants
                .Where(p => p.UserId == user.Id)
                .GroupBy(p => p.Type)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => new KeyValuePair<string,int>(x.Type, x.Count))
                .ToListAsync();

            TasksByType = await _context.GardenTasks
                .Include(t => t.Plant)
                .Where(t => t.Plant != null && t.Plant.UserId == user.Id)
                .GroupBy(t => t.Type)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => new KeyValuePair<string,int>(x.Type, x.Count))
                .ToListAsync();

            var top = await _context.GardenTasks
                .Include(t => t.Plant)
                .Where(t => t.Plant != null && t.Plant.UserId == user.Id)
                .GroupBy(t => t.PlantId)
                .Select(g => new { PlantId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var plantIds = top.Select(t => t.PlantId).ToList();
            var plants = await _context.Plants.Where(p => plantIds.Contains(p.Id)).ToListAsync();
            TopPlants = top.Select(t => new TopPlantDto { Plant = plants.First(p => p.Id == t.PlantId), TaskCount = t.Count }).ToList();

            return Page();
        }
    }
}
