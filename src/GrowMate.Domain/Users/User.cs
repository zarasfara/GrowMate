using Microsoft.AspNetCore.Identity;

namespace GrowMate.Domain.Users;

/// <summary>
/// Пользователь системы GrowMate
/// </summary>
public sealed class User : IdentityUser
{
    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}

