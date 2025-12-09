using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

/// <summary>
/// Repositorio específico para la entidad Employee
/// </summary>
public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByDocumentNumberAsync(string documentNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber);
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.PersonalEmail == email || e.CorporateEmail == email);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
    {
        return await _dbSet
            .Where(e => e.DepartmentId == departmentId && e.IsActive)
            .Include(e => e.Department)
            .Include(e => e.JobPosition)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByJobPositionAsync(int jobPositionId)
    {
        return await _dbSet
            .Where(e => e.JobPositionId == jobPositionId && e.IsActive)
            .Include(e => e.Department)
            .Include(e => e.JobPosition)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status)
    {
        return await _dbSet
            .Where(e => e.Status == status && e.IsActive)
            .Include(e => e.Department)
            .Include(e => e.JobPosition)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
    {
        return await _dbSet
            .Where(e => e.Status == EmployeeStatus.Activo && e.IsActive)
            .Include(e => e.Department)
            .Include(e => e.JobPosition)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.JobPosition)
            .Include(e => e.EducationLevels.Where(el => el.IsActive))
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(e => e.IsActive && (
                e.DocumentNumber.ToLower().Contains(lowerSearchTerm) ||
                e.FirstName.ToLower().Contains(lowerSearchTerm) ||
                e.LastName.ToLower().Contains(lowerSearchTerm) ||
                (e.PersonalEmail != null && e.PersonalEmail.ToLower().Contains(lowerSearchTerm))
            ))
            .Include(e => e.Department)
            .Include(e => e.JobPosition)
            .ToListAsync();
    }

    public async Task<bool> DocumentNumberExistsAsync(string documentNumber, int? excludeEmployeeId = null)
    {
        var query = _dbSet.Where(e => e.DocumentNumber == documentNumber);

        if (excludeEmployeeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeEmployeeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null)
    {
        var query = _dbSet.Where(e => e.PersonalEmail == email || e.CorporateEmail == email);

        if (excludeEmployeeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeEmployeeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Dictionary<int, int>> GetEmployeeCountByDepartmentAsync()
    {
        return await _dbSet
            .Where(e => e.IsActive)
            .GroupBy(e => e.DepartmentId)
            .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DepartmentId, x => x.Count);
    }

    public async Task<Dictionary<EmployeeStatus, int>> GetEmployeeCountByStatusAsync()
    {
        return await _dbSet
            .Where(e => e.IsActive)
            .GroupBy(e => e.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    public async Task<IEnumerable<Employee>> GetHiredBetweenAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.HireDate >= startDate && e.HireDate <= endDate && e.IsActive)
            .Include(e => e.Department)
            .Include(e => e.JobPosition)
            .OrderBy(e => e.HireDate)
            .ToListAsync();
    }
}
