using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartamentosController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }
}
