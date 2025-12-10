using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TalentoPlus.Application.DTOs.AI;
using TalentoPlus.Application.Services.Interfaces;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class GeminiService : IAIService
{
    private readonly IConfiguration _configuration;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly HttpClient _httpClient;

    public GeminiService(
        IConfiguration configuration, 
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Processes a natural language question and returns a data-driven answer.
    /// </summary>
    public async Task<AIResponseDto> ProcessQuestionAsync(string question)
    {
        var intent = await GetIntentFromQuestionAsync(question);

        if (intent == null)
        {
            return new AIResponseDto { Success = false, Answer = "No pude entender tu pregunta. Intenta preguntar sobre 'cuántos empleados hay', 'promedio de salarios', o 'lista de empleados en IT'." };
        }

        return await ExecuteQueryAsync(intent);
    }

    private async Task<AIIntentDto?> GetIntentFromQuestionAsync(string question)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (!string.IsNullOrEmpty(apiKey) && apiKey != "YOUR_API_KEY_HERE")
        {
            // TODO: Implementar llamada real a Gemini API aquí si el usuario provee la key
            // Por ahora, usaremos el intérprete local para asegurar que funcione la demo
            return LocalIntentInterpreter(question);
        }
        else
        {
            // Fallback: Intérprete local basado en reglas
            return LocalIntentInterpreter(question);
        }
    }

    private AIIntentDto LocalIntentInterpreter(string question)
    {
        var q = question.ToLower();
        var intent = new AIIntentDto();

        // 1. Detect Query Type
        if (q.Contains("mayor salario") || q.Contains("gana mas") || q.Contains("sueldo mas alto") || q.Contains("mejor pagado"))
        {
            intent.QueryType = "max";
            intent.TargetField = "salary";
            intent.Explanation = "Buscar salario más alto";
        }
        else if (q.Contains("menor salario") || q.Contains("gana menos") || q.Contains("sueldo mas bajo") || q.Contains("peor pagado"))
        {
            intent.QueryType = "min";
            intent.TargetField = "salary";
            intent.Explanation = "Buscar salario más bajo";
        }
        else if (q.Contains("promedio") || q.Contains("media"))
        {
            intent.QueryType = "average";
            intent.TargetField = "salary";
            intent.Explanation = "Calcular promedio de salario";
        }
        else if (q.Contains("suma") || q.Contains("total salario") || q.Contains("costo") || q.Contains("nomina"))
        {
            intent.QueryType = "sum";
            intent.TargetField = "salary";
            intent.Explanation = "Sumar salarios";
        }
        else if (q.Contains("cuantos") || q.Contains("cantidad") || q.Contains("numero") || q.Contains("total"))
        {
            intent.QueryType = "count";
            intent.Explanation = "Contar empleados";
        }
        else if (q.Contains("lista") || q.Contains("quienes") || q.Contains("mostrar") || q.Contains("cuales"))
        {
            intent.QueryType = "list";
            intent.Explanation = "Listar empleados";
        }
        else
        {
            // Default
            intent.QueryType = "count";
            intent.Explanation = "Contar empleados (por defecto)";
        }

        // 2. Detect Filters
        
        // Status
        if (q.Contains("activo")) intent.Filters.Status = "Activo";
        if (q.Contains("inactivo") || q.Contains("retirado")) intent.Filters.Status = "Retirado";
        
        // Gender
        if (q.Contains("mujer") || q.Contains("femenino") || q.Contains("chicas")) intent.Filters.Gender = "Femenino";
        if (q.Contains("hombre") || q.Contains("masculino") || q.Contains("chicos")) intent.Filters.Gender = "Masculino";

        // Temporal
        if (q.Contains("nuevo") || q.Contains("reciente") || q.Contains("ingresaron") || q.Contains("ultimo mes")) 
            intent.Filters.IsRecent = true;
            
        if (q.Contains("cumple") || q.Contains("cumpleaños") || q.Contains("santo")) 
            intent.Filters.IsBirthdayMonth = true;

        // Departments (Simple simulation)
        if (q.Contains("sistemas") || q.Contains("ti") || q.Contains("tecnologia")) intent.Filters.Department = "Tecnología";
        if (q.Contains("rrhh") || q.Contains("humanos")) intent.Filters.Department = "Recursos Humanos";
        if (q.Contains("ventas") || q.Contains("comercial")) intent.Filters.Department = "Ventas";
        if (q.Contains("marketing") || q.Contains("mercadeo")) intent.Filters.Department = "Marketing";
        if (q.Contains("logistica")) intent.Filters.Department = "Logística";
        if (q.Contains("contabilidad") || q.Contains("financiera")) intent.Filters.Department = "Contabilidad";

        return intent;
    }

    private async Task<AIResponseDto> ExecuteQueryAsync(AIIntentDto intent)
    {
        var response = new AIResponseDto();
        
        // Get employees (base)
        var employees = await _employeeRepository.GetAllAsync();

        // --- APPLY IN-MEMORY FILTERS ---

        // Department
        if (!string.IsNullOrEmpty(intent.Filters.Department))
        {
            employees = employees.Where(e => e.Department != null && 
                e.Department.Name.Contains(intent.Filters.Department, StringComparison.OrdinalIgnoreCase));
        }

        // Status (Note: GetAllAsync already fetches active ones by default in the modified repo, 
        // but if we had access to all, we would filter here)
        if (!string.IsNullOrEmpty(intent.Filters.Status))
        {
            if (intent.Filters.Status == "Retirado")
            {
                response.Answer = "Nota: Actualmente solo consulto empleados activos. ";
            }
        }

        // Gender
        if (!string.IsNullOrEmpty(intent.Filters.Gender))
        {
            employees = employees.Where(e => !string.IsNullOrEmpty(e.Gender) && e.Gender.Equals(intent.Filters.Gender, StringComparison.OrdinalIgnoreCase));
        }

        // Recent (last 30 days)
        if (intent.Filters.IsRecent)
        {
            var limitDate = DateTime.Now.AddDays(-30);
            employees = employees.Where(e => e.HireDate >= limitDate);
        }

        // Birthday (this month)
        if (intent.Filters.IsBirthdayMonth)
        {
            var currentMonth = DateTime.Now.Month;
            employees = employees.Where(e => e.DateOfBirth.Month == currentMonth);
        }

        // --- EXECUTE ACTION ---

        if (!employees.Any())
        {
            response.Answer += "No encontré ningún empleado que coincida con esos criterios.";
            return response;
        }

        switch (intent.QueryType)
        {
            case "count":
                var count = employees.Count();
                response.Answer += $"Hay un total de {count} empleados encontrados.";
                response.Data = count;
                break;

            case "average":
                if (intent.TargetField == "salary")
                {
                    var avg = employees.Average(e => e.Salary);
                    response.Answer += $"El salario promedio es de {avg:C2}.";
                    response.Data = avg;
                }
                break;

            case "sum":
                if (intent.TargetField == "salary")
                {
                    var sum = employees.Sum(e => e.Salary);
                    response.Answer += $"El costo total de nómina mensual para estos empleados es {sum:C2}.";
                    response.Data = sum;
                }
                break;

            case "max":
                if (intent.TargetField == "salary")
                {
                    var maxEmp = employees.OrderByDescending(e => e.Salary).First();
                    response.Answer += $"El empleado con mayor salario es {maxEmp.FirstName} {maxEmp.LastName} ({maxEmp.JobPosition?.Title}) con {maxEmp.Salary:C2}.";
                    response.Data = maxEmp;
                }
                break;
                
            case "min":
                if (intent.TargetField == "salary")
                {
                    var minEmp = employees.OrderBy(e => e.Salary).First();
                    response.Answer += $"El empleado con menor salario es {minEmp.FirstName} {minEmp.LastName} ({minEmp.JobPosition?.Title}) con {minEmp.Salary:C2}.";
                    response.Data = minEmp;
                }
                break;

            case "list":
                var list = employees.Take(10).Select(e => $"{e.FirstName} {e.LastName}").ToList();
                response.Answer += $"Encontré {employees.Count()} empleados. Aquí tienes los primeros: {string.Join(", ", list)}.";
                response.Data = list;
                break;

            default:
                response.Answer = "Entendí la pregunta, pero no supe qué operación matemática realizar.";
                break;
        }

        return response;
    }
}
