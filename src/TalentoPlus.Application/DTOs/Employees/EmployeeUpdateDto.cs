namespace TalentoPlus.Application.DTOs.Employees;

public class EmployeeUpdateDto : EmployeeCreateDto
{
    public int Id { get; set; }
    public DateTime? TerminationDate { get; set; }
    public bool IsActive { get; set; }
}
