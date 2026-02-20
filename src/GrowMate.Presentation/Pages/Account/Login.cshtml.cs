using GrowMate.Domain.Users;
using GrowMate.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrowMate.Presentation.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public LoginModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty] public LoginViewModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData] public string? ErrorMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage)) ModelState.AddModelError(string.Empty, ErrorMessage);

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, true);

        if (result.Succeeded) return LocalRedirect(ReturnUrl ?? "/");

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Аккаунт заблокирован из-за множественных неудачных попыток входа.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
        return Page();
    }
}