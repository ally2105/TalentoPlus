using TalentoPlus.Application.DTOs.AI;

namespace TalentoPlus.Application.Services.Interfaces;

public interface IAIService
{
    /// <summary>
    /// Procesa una pregunta en lenguaje natural y devuelve una respuesta basada en datos reales.
    /// </summary>
    Task<AIResponseDto> ProcessQuestionAsync(string question);
}

public class AIResponseDto
{
    public string Answer { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string ChartType { get; set; } = "none"; // "bar", "pie", "none"
    public bool Success { get; set; } = true;
}
