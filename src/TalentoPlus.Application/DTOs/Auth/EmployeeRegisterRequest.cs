using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Application.DTOs.Auth;

public class EmployeeRegisterRequest
{
    [Required]
    public string DocumentNumber { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public int DepartmentId { get; set; }
    
    [Required]
    public int JobPositionId { get; set; }
}
