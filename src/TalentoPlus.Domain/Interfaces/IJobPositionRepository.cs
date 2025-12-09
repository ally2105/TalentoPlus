using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IJobPositionRepository : IRepository<JobPosition>
{
    Task<IEnumerable<JobPosition>> GetByDepartmentAsync(int departmentId);
}
