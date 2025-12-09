using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services.Interfaces;

namespace TalentoPlus.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class DashboardController : Controller
{
    private readonly IEmployeeService _employeeService;

    public DashboardController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _employeeService.GetDashboardStatsAsync();
        return View(stats);
    }
}
