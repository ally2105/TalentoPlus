using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class JobPositionRepository : Repository<JobPosition>, IJobPositionRepository
{
    public JobPositionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<JobPosition>> GetByDepartmentAsync(int departmentId)
    {
        return await _dbSet
            .Where(j => j.DepartmentId == departmentId && j.IsActive)
            .OrderBy(j => j.Title)
            .ToListAsync();
    }
}
