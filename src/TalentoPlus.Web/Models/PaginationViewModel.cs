namespace TalentoPlus.Web.Models;

/// <summary>
/// ViewModel para los controles de paginación
/// </summary>
public class PaginationViewModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public string? SearchTerm { get; set; }
    public string? ActionName { get; set; }
    public string? ControllerName { get; set; }

    public PaginationViewModel()
    {
        ActionName = "Index";
    }
}
