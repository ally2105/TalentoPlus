using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

/// <summary>
/// Repositorio específico para la entidad Department
/// </summary>
public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Department?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.Code == code);
    }

    public async Task<Department?> GetByIdWithEmployeesAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Employees.Where(e => e.IsActive))
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Department?> GetByIdWithJobPositionsAsync(int id)
    {
        return await _dbSet
            .Include(d => d.JobPositions.Where(j => j.IsActive))
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Department?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Employees.Where(e => e.IsActive))
            .Include(d => d.JobPositions.Where(j => j.IsActive))
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Department>> SearchByNameAsync(string name)
    {
        var lowerName = name.ToLower();

        return await _dbSet
            .Where(d => d.IsActive && d.Name.ToLower().Contains(lowerName))
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeDepartmentId = null)
    {
        var query = _dbSet.Where(d => d.Code == code);

        if (excludeDepartmentId.HasValue)
        {
            query = query.Where(d => d.Id != excludeDepartmentId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<(Department Department, int EmployeeCount)>> GetDepartmentsWithEmployeeCountAsync()
    {
        return await _dbSet
            .Where(d => d.IsActive)
            .Select(d => new
            {
                Department = d,
                EmployeeCount = d.Employees.Count(e => e.IsActive)
            })
            .ToListAsync()
            .ContinueWith(task => task.Result.Select(x => (x.Department, x.EmployeeCount)));
    }

    public async Task<bool> HasEmployeesAsync(int departmentId)
    {
        return await _context.Employees
            .AnyAsync(e => e.DepartmentId == departmentId && e.IsActive);
    }

    public async Task<bool> HasJobPositionsAsync(int departmentId)
    {
        return await _context.JobPositions
            .AnyAsync(j => j.DepartmentId == departmentId && j.IsActive);
    }
}
