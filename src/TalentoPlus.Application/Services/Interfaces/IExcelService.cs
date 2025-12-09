using TalentoPlus.Application.DTOs.Common;

namespace TalentoPlus.Application.Services.Interfaces;

public interface IExcelService
{
    Task<ImportResultDto> ImportEmployeesAsync(Stream fileStream);
}
