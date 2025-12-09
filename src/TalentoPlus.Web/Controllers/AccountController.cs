using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Web.Models;

namespace TalentoPlus.Web.Controllers;

/// <summary>
/// Controlador para la gestión de autenticación de usuarios
/// </summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Muestra el formulario de inicio de sesión
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    /// <summary>
    /// Procesa el inicio de sesión
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Buscar el usuario por email primero
        var user = await _userManager.FindByEmailAsync(model.Email);
        
        if (user == null)
        {
            _logger.LogWarning($"Intento de login fallido: usuario no encontrado para email {model.Email}");
            ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos");
            return View(model);
        }

        // Usar el UserName para el login (no el email)
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"Usuario {model.Email} inició sesión exitosamente");

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning($"Cuenta bloqueada para: {model.Email}");
            ModelState.AddModelError(string.Empty, "Cuenta bloqueada. Intenta de nuevo en 15 minutos.");
            return View(model);
        }

        if (result.RequiresTwoFactor)
        {
            _logger.LogInformation($"Se requiere segundo factor para: {model.Email}");
            ModelState.AddModelError(string.Empty, "Se requiere autenticación de dos factores");
            return View(model);
        }

        _logger.LogWarning($"Intento de login fallido: contraseña incorrecta para {model.Email}");
        ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos");
        return View(model);
    }

    /// <summary>
    /// Cierra la sesión del usuario
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userName = User.Identity?.Name;
        await _signInManager.SignOutAsync();
        _logger.LogInformation($"Usuario {userName} cerró sesión");
        return RedirectToAction("Login", "Account");
    }

    /// <summary>
    /// Muestra la página de acceso denegado
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
