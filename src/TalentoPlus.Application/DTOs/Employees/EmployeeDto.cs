using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.DTOs.Employees;

public class EmployeeDto
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? SecondLastName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string? Gender { get; set; }
    public string PersonalEmail { get; set; } = string.Empty;
    public string? CorporateEmail { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternativePhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string Country { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? ProfessionalProfile { get; set; }
    public double YearsOfService { get; set; }
    
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    
    public int JobPositionId { get; set; }
    public string JobPositionTitle { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
