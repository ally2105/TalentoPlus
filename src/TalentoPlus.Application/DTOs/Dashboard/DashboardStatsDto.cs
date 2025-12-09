using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; } // Retirados, etc.
    public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
    public Dictionary<string, int> EmployeesByStatus { get; set; } = new();
    
    // Métricas adicionales para gráficas
    public List<string> DepartmentLabels => EmployeesByDepartment.Keys.ToList();
    public List<int> DepartmentValues => EmployeesByDepartment.Values.ToList();
}
