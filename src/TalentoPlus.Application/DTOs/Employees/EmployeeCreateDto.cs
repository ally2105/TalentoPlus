using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.DTOs.Employees;

public class EmployeeCreateDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentType { get; set; } = "CC";
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? SecondLastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string PersonalEmail { get; set; } = string.Empty;
    public string? CorporateEmail { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternativePhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string Country { get; set; } = "Colombia";
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Activo;
    public string? ProfessionalProfile { get; set; }
    public int DepartmentId { get; set; }
    public int JobPositionId { get; set; }
}
