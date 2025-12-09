using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.DTOs.Employees;

public class EmployeeListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string PersonalEmail { get; set; } = string.Empty;
    public string? CorporateEmail { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string JobPositionTitle { get; set; } = string.Empty;
    public EmployeeStatus Status { get; set; }
    public DateTime HireDate { get; set; }
}
