using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services.Interfaces;

namespace TalentoPlus.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class DashboardController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IAIService _aiService;

    public DashboardController(IEmployeeService employeeService, IAIService aiService)
    {
        _employeeService = employeeService;
        _aiService = aiService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _employeeService.GetDashboardStatsAsync();
        return View(stats);
    }

    [HttpPost]
    public async Task<IActionResult> AskAI([FromBody] AIQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest("La pregunta no puede estar vacía.");

        var response = await _aiService.ProcessQuestionAsync(request.Question);
        return Ok(response);
    }
}

public class AIQueryRequest
{
    public string Question { get; set; }
}
