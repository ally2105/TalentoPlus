using ClosedXML.Excel;
using Moq;
using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Services;
using Xunit;

namespace TalentoPlus.Tests.Unit;

public class ExcelServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockEmployeeRepo;
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepo;
    private readonly Mock<IJobPositionRepository> _mockJobPositionRepo;
    private readonly ExcelService _excelService;

    public ExcelServiceTests()
    {
        _mockEmployeeRepo = new Mock<IEmployeeRepository>();
        _mockDepartmentRepo = new Mock<IDepartmentRepository>();
        _mockJobPositionRepo = new Mock<IJobPositionRepository>();

        _excelService = new ExcelService(
            _mockEmployeeRepo.Object,
            _mockDepartmentRepo.Object,
            _mockJobPositionRepo.Object
        );
    }

    [Fact]
    public async Task ImportEmployeesAsync_ValidFile_ShouldImportEmployees()
    {
        // Arrange
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Empleados");
        
        // Headers
        worksheet.Cell(1, 1).Value = "Documento";
        worksheet.Cell(1, 2).Value = "Tipo Documento";
        worksheet.Cell(1, 3).Value = "Nombres";
        worksheet.Cell(1, 4).Value = "Apellidos";
        worksheet.Cell(1, 5).Value = "Email Personal";
        worksheet.Cell(1, 6).Value = "Departamento";
        worksheet.Cell(1, 7).Value = "Cargo";
        worksheet.Cell(1, 8).Value = "Fecha Ingreso";
        worksheet.Cell(1, 9).Value = "Salario";

        // Data
        worksheet.Cell(2, 1).Value = "123456789";
        worksheet.Cell(2, 2).Value = "CC";
        worksheet.Cell(2, 3).Value = "Juan";
        worksheet.Cell(2, 4).Value = "Perez";
        worksheet.Cell(2, 5).Value = "juan.perez@test.com";
        worksheet.Cell(2, 6).Value = "Tecnología";
        worksheet.Cell(2, 7).Value = "Desarrollador";
        worksheet.Cell(2, 8).Value = DateTime.Now.ToString("yyyy-MM-dd");
        worksheet.Cell(2, 9).Value = 5000000;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        // Mock Repos
        _mockDepartmentRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Department>()); // Simulate no departments exist initially
        
        _mockDepartmentRepo.Setup(r => r.AddAsync(It.IsAny<Department>()))
            .Callback<Department>(d => d.Id = 1)
            .ReturnsAsync((Department d) => d);

        _mockJobPositionRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<JobPosition>());

        _mockJobPositionRepo.Setup(r => r.AddAsync(It.IsAny<JobPosition>()))
            .Callback<JobPosition>(j => j.Id = 1)
            .ReturnsAsync((JobPosition j) => j);

        _mockEmployeeRepo.Setup(r => r.DocumentNumberExistsAsync(It.IsAny<string>(), null))
            .ReturnsAsync(false);
            
        _mockEmployeeRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null))
            .ReturnsAsync(false);
            
        _mockEmployeeRepo.Setup(r => r.AddAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);

        // Act
        var result = await _excelService.ImportEmployeesAsync(stream);

        // Assert
        Assert.Equal(1, result.SuccessfulImports);
        Assert.Equal(0, result.FailedImports);
        _mockEmployeeRepo.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }
}
