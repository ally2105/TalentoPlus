using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Services;
using Xunit;

namespace TalentoPlus.Tests.Unit.Services;

public class GeminiServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IEmployeeRepository> _mockRepo;
    private readonly Mock<IDepartmentRepository> _mockDeptRepo;
    private readonly Mock<HttpClient> _mockHttp; // No usado en local, pero requerido por constructor
    private readonly GeminiService _service;

    public GeminiServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockRepo = new Mock<IEmployeeRepository>();
        _mockDeptRepo = new Mock<IDepartmentRepository>();
        _mockHttp = new Mock<HttpClient>(); // HttpClient es difícil de mockear así, pero pasaremos null o dummy si es posible, o mejor un HttpClient real que no se use.
        
        // Configurar para usar intérprete local (sin API Key)
        _mockConfig.Setup(c => c["Gemini:ApiKey"]).Returns("");

        _service = new GeminiService(
            _mockConfig.Object, 
            _mockRepo.Object, 
            _mockDeptRepo.Object, 
            new HttpClient());
    }

    [Fact]
    public async Task ProcessQuestionAsync_Should_Return_Count_When_Asked_Cuantos()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "A", IsActive = true },
            new Employee { Id = 2, FirstName = "B", IsActive = true }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

        // Act
        var result = await _service.ProcessQuestionAsync("¿Cuántos empleados hay?");

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(2);
        result.Answer.Should().Contain("2 empleados");
    }

    [Fact]
    public async Task ProcessQuestionAsync_Should_Filter_By_Gender()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "Ana", Gender = "Femenino", IsActive = true },
            new Employee { Id = 2, FirstName = "Luis", Gender = "Masculino", IsActive = true }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

        // Act
        var result = await _service.ProcessQuestionAsync("¿Cuántas mujeres hay?");

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(1); // Solo Ana
    }

    [Fact]
    public async Task ProcessQuestionAsync_Should_Calculate_Average_Salary()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, Salary = 1000, IsActive = true },
            new Employee { Id = 2, Salary = 2000, IsActive = true }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

        // Act
        var result = await _service.ProcessQuestionAsync("¿Cuál es el salario promedio?");

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(1500);
    }
}
