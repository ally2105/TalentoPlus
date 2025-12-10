using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentoPlus.Application.DTOs.Auth;
using TalentoPlus.Application.DTOs.Employees;
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

    /// <summary>
    /// Authenticates an employee and returns a JWT token
    /// </summary>
    /// <param name="request">Access credentials</param>
    /// <returns>JWT token and basic data</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _authService.LoginAsync(request);
        if (response == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(response);
    }

    /// <summary>
    /// Registers a new employee in the system
    /// </summary>
    /// <param name="request">Employee data</param>
    /// <returns>Confirmation message</returns>
    [HttpPost("registro")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Gets the profile of the authenticated employee
    /// </summary>
    /// <returns>Employee data</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Downloads the authenticated employee's Resume in PDF
    /// </summary>
    /// <returns>PDF File</returns>
    [Authorize]
    [HttpGet("me/resume")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
