namespace TalentoPlus.Application.DTOs.AI;

public class AIIntentDto
{
    // Tipo de consulta: "count", "list", "average", "sum"
    public string QueryType { get; set; } = string.Empty;
    
    // Entidad principal: "employee" (por ahora solo esa)
    public string Entity { get; set; } = "employee";
    
    // Filtros detectados
    public AIFiltersDto Filters { get; set; } = new();
    
    // Campo objetivo para operaciones numéricas (ej: "salary")
    public string TargetField { get; set; } = string.Empty;
    
    // Respuesta explicativa generada por la IA
    public string Explanation { get; set; } = string.Empty;
}

public class AIFiltersDto
{
    public string? Department { get; set; }
    public string? JobPosition { get; set; }
    public string? Status { get; set; } // "Activo", "Inactivo"
    public string? Name { get; set; }
    public string? Gender { get; set; } // "Masculino", "Femenino"
    public bool IsRecent { get; set; } // Últimos 30 días
    public bool IsBirthdayMonth { get; set; } // Cumpleaños este mes
}
