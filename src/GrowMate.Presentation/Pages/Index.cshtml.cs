using GrowMate.Domain.GardenBeds;
using GrowMate.Domain.GardenTasks;
using GrowMate.Domain.Plants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrowMate.Presentation.Pages;

[Authorize]
public class IndexModel : PageModel
{
    // TODO: Внедрить зависимости для работы с БД
    // private readonly ApplicationContext _context;
    // private readonly UserManager<User> _userManager;
    
    // public IndexModel(ApplicationContext context, UserManager<User> userManager)
    // {
    //     _context = context;
    //     _userManager = userManager;
    // }
    
    // Заглушки данных для демонстрации
    public List<GardenBed> GardenBeds { get; set; } = new();
    public List<Plant> RecentPlants { get; set; } = new();
    public List<GardenTask> UpcomingTasks { get; set; } = new();
    
    // Статистика
    public int TotalGardenBeds => GardenBeds.Count;
    public int TotalPlants => RecentPlants.Count;
    public int PendingTasks => UpcomingTasks.Count(t => !t.IsCompleted);
    public int CompletedTasksToday => UpcomingTasks.Count(t => t.IsCompleted && t.DueDate.Date == DateTime.Today);
    
    // Дополнительная статистика (можно использовать в будущем)
    public int OverdueTasks => UpcomingTasks.Count(t => !t.IsCompleted && t.DueDate.Date < DateTime.Today);
    public int TasksToday => UpcomingTasks.Count(t => t.DueDate.Date == DateTime.Today);
    public int PlantsByType(PlantType type) => RecentPlants.Count(p => p.Type == type);

    public void OnGet()
    {
        // TODO: Загрузить реальные данные из базы данных для текущего пользователя
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // await LoadDataFromDatabase(userId);
        
        LoadMockData();
    }
    
    // TODO: Реализовать метод загрузки данных из БД
    // private async Task LoadDataFromDatabase(string userId)
    // {
    //     GardenBeds = await _context.GardenBeds
    //         .Where(g => g.UserId == userId)
    //         .ToListAsync();
    //     
    //     RecentPlants = await _context.Plants
    //         .Where(p => p.UserId == userId)
    //         .Include(p => p.GardenBed)
    //         .OrderByDescending(p => p.PlantingDate)
    //         .Take(12)
    //         .ToListAsync();
    //     
    //     UpcomingTasks = await _context.GardenTasks
    //         .Include(t => t.Plant)
    //         .Where(t => t.Plant.UserId == userId && t.DueDate >= DateTime.Today.AddDays(-7))
    //         .OrderBy(t => t.DueDate)
    //         .ToListAsync();
    // }

    private void LoadMockData()
    {
        // TODO: Заменить на загрузку из БД
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // GardenBeds = await _context.GardenBeds.Where(g => g.UserId == userId).ToListAsync();
        
        // Заглушки грядок - демонстрация разных типов
        GardenBeds = new List<GardenBed>
        {
            new GardenBed { Id = 1, Name = "Теплица №1", Type = GardenBedType.Greenhouse, UserId = "user1" },
            new GardenBed { Id = 2, Name = "Грядка с томатами", Type = GardenBedType.OpenGround, UserId = "user1" },
            new GardenBed { Id = 3, Name = "Контейнер с зеленью", Type = GardenBedType.Container, UserId = "user1" },
            new GardenBed { Id = 4, Name = "Теплица №2 (огурцы)", Type = GardenBedType.Greenhouse, UserId = "user1" },
            new GardenBed { Id = 5, Name = "Открытая грядка с клубникой", Type = GardenBedType.OpenGround, UserId = "user1" }
        };

        // Заглушки растений - демонстрация всех типов растений
        RecentPlants = new List<Plant>
        {
            // Овощи
            new Plant { Id = 1, Name = "Томат", Variety = "Черри", Type = PlantType.Vegetable, PlantingDate = DateTime.Now.AddDays(-30), UserId = "user1", GardenBedId = 1 },
            new Plant { Id = 2, Name = "Огурец", Variety = "Герман F1", Type = PlantType.Vegetable, PlantingDate = DateTime.Now.AddDays(-25), UserId = "user1", GardenBedId = 4 },
            new Plant { Id = 3, Name = "Перец", Variety = "Болгарский сладкий", Type = PlantType.Vegetable, PlantingDate = DateTime.Now.AddDays(-20), UserId = "user1", GardenBedId = 1 },
            new Plant { Id = 4, Name = "Баклажан", Variety = "Черный красавец", Type = PlantType.Vegetable, PlantingDate = DateTime.Now.AddDays(-35), UserId = "user1", GardenBedId = 1 },
            
            // Зелень
            new Plant { Id = 5, Name = "Базилик", Variety = "Фиолетовый", Type = PlantType.Herb, PlantingDate = DateTime.Now.AddDays(-15), UserId = "user1", GardenBedId = 3 },
            new Plant { Id = 6, Name = "Петрушка", Type = PlantType.Herb, PlantingDate = DateTime.Now.AddDays(-12), UserId = "user1", GardenBedId = 3 },
            new Plant { Id = 7, Name = "Укроп", Type = PlantType.Herb, PlantingDate = DateTime.Now.AddDays(-10), UserId = "user1", GardenBedId = 3 },
            
            // Ягоды
            new Plant { Id = 8, Name = "Клубника", Variety = "Виктория", Type = PlantType.Berry, PlantingDate = DateTime.Now.AddDays(-60), UserId = "user1", GardenBedId = 5 },
            new Plant { Id = 9, Name = "Земляника", Variety = "Альбион", Type = PlantType.Berry, PlantingDate = DateTime.Now.AddDays(-50), UserId = "user1", GardenBedId = 5 },
            
            // Цветы
            new Plant { Id = 10, Name = "Петуния", Variety = "Разноцветная", Type = PlantType.Flower, PlantingDate = DateTime.Now.AddDays(-40), UserId = "user1", GardenBedId = 3 },
            new Plant { Id = 11, Name = "Бархатцы", Type = PlantType.Flower, PlantingDate = DateTime.Now.AddDays(-35), UserId = "user1", GardenBedId = 2 },
            
            // Фрукты (саженцы)
            new Plant { Id = 12, Name = "Яблоня", Variety = "Антоновка", Type = PlantType.Fruit, PlantingDate = DateTime.Now.AddDays(-100), UserId = "user1", GardenBedId = 2 }
        };

        // Заглушки задач - демонстрация разных состояний и типов
        UpcomingTasks = new List<GardenTask>
        {
            // Просроченные задачи
            new GardenTask { Id = 1, Title = "Полить томаты в теплице", Type = TaskType.Watering, DueDate = DateTime.Today.AddDays(-2), IsCompleted = false, PlantId = 1 },
            new GardenTask { Id = 2, Title = "Подкормить перец", Type = TaskType.Fertilizing, DueDate = DateTime.Today.AddDays(-1), IsCompleted = false, PlantId = 3 },
            
            // Задачи на сегодня
            new GardenTask { Id = 3, Title = "Полить огурцы", Type = TaskType.Watering, DueDate = DateTime.Today, IsCompleted = false, PlantId = 2 },
            new GardenTask { Id = 4, Title = "Обработать баклажаны от тли", Type = TaskType.Treatment, DueDate = DateTime.Today, IsCompleted = false, PlantId = 4 },
            new GardenTask { Id = 5, Title = "Полить базилик", Type = TaskType.Watering, DueDate = DateTime.Today, IsCompleted = true, PlantId = 5 },
            new GardenTask { Id = 6, Title = "Подкормить клубнику", Type = TaskType.Fertilizing, DueDate = DateTime.Today, IsCompleted = true, PlantId = 8 },
            
            // Будущие задачи
            new GardenTask { Id = 7, Title = "Пересадить петунию", Type = TaskType.Other, DueDate = DateTime.Today.AddDays(1), IsCompleted = false, PlantId = 10 },
            new GardenTask { Id = 8, Title = "Полить укроп и петрушку", Type = TaskType.Watering, DueDate = DateTime.Today.AddDays(1), IsCompleted = false, PlantId = 6 },
            new GardenTask { Id = 9, Title = "Подкормить томаты комплексным удобрением", Type = TaskType.Fertilizing, DueDate = DateTime.Today.AddDays(2), IsCompleted = false, PlantId = 1 },
            new GardenTask { Id = 10, Title = "Обработать огурцы от мучнистой росы", Type = TaskType.Treatment, DueDate = DateTime.Today.AddDays(3), IsCompleted = false, PlantId = 2 },
            new GardenTask { Id = 11, Title = "Прополка грядки с клубникой", Type = TaskType.Other, DueDate = DateTime.Today.AddDays(4), IsCompleted = false, PlantId = 8 },
            new GardenTask { Id = 12, Title = "Полить яблоню", Type = TaskType.Watering, DueDate = DateTime.Today.AddDays(5), IsCompleted = false, PlantId = 12 },
            new GardenTask { Id = 13, Title = "Подкормить землянику органикой", Type = TaskType.Fertilizing, DueDate = DateTime.Today.AddDays(7), IsCompleted = false, PlantId = 9 }
        };
    }
}

