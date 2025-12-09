using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentoPlus.Application.DTOs.Auth;
using TalentoPlus.Application.Services.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IAuthService _authService;

    public EmpleadosController(IEmployeeService employeeService, IAuthService authService)
    {
        _employeeService = employeeService;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _authService.LoginAsync(request);
        if (response == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(response);
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Register([FromBody] EmployeeRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _employeeService.RegisterAsync(request);
            return StatusCode(201, new { message = "Empleado registrado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        if (!int.TryParse(userIdClaim.Value, out int employeeId))
            return BadRequest("ID de empleado inválido en token");

        var employee = await _employeeService.GetByIdAsync(employeeId);
        if (employee == null) return NotFound("Empleado no encontrado");

        return Ok(employee);
    }

    [Authorize]
    [HttpGet("me/resume")]
    public async Task<IActionResult> DownloadMyResume([FromServices] IPdfService pdfService)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        if (!int.TryParse(userIdClaim.Value, out int employeeId))
            return BadRequest("ID de empleado inválido en token");

        var employee = await _employeeService.GetByIdAsync(employeeId);
        if (employee == null) return NotFound("Empleado no encontrado");

        var pdfBytes = pdfService.GenerateEmployeeResume(employee);
        return File(pdfBytes, "application/pdf", $"CV_{employee.FirstName}_{employee.LastName}.pdf");
    }
}
