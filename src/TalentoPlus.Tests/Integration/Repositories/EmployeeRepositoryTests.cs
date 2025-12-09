using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;
using Xunit;

namespace TalentoPlus.Tests.Integration.Repositories;

public class EmployeeRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única por test
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EmployeeRepository(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Employee()
    {
        // Arrange
        var employee = new Employee
        {
            DocumentNumber = "123",
            FirstName = "Test",
            LastName = "User",
            PersonalEmail = "test@test.com",
            DepartmentId = 1,
            JobPositionId = 1,
            Status = EmployeeStatus.Activo,
            IsActive = true
        };

        // Act
        await _repository.AddAsync(employee);
        await _context.SaveChangesAsync();

        // Assert
        var savedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.DocumentNumber == "123");
        savedEmployee.Should().NotBeNull();
        savedEmployee!.FirstName.Should().Be("Test");
    }

    [Fact]
    public async Task GetActiveEmployeesAsync_Should_Return_Only_Active()
    {
        // Arrange
        // Crear dependencias requeridas
        var dept = new Department { Name = "Test Dept", Code = "TD" };
        var job = new JobPosition { Title = "Test Job", Department = dept };
        _context.Departments.Add(dept);
        _context.JobPositions.Add(job);
        await _context.SaveChangesAsync();

        _context.Employees.Add(new Employee 
        { 
            DocumentNumber = "1", 
            FirstName = "Active", 
            IsActive = true, 
            Status = EmployeeStatus.Activo,
            DepartmentId = dept.Id,
            JobPositionId = job.Id
        });
        _context.Employees.Add(new Employee 
        { 
            DocumentNumber = "2", 
            FirstName = "Inactive", 
            IsActive = false, 
            Status = EmployeeStatus.Retirado,
            DepartmentId = dept.Id,
            JobPositionId = job.Id
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveEmployeesAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("Active");
    }
}
