namespace TalentoPlus.Application.DTOs.Common;

public class ImportResultDto
{
    public int TotalProcessed { get; set; }
    public int SuccessfulImports { get; set; }
    public int FailedImports { get; set; }
    public List<string> Errors { get; set; } = new();
}
