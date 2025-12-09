using ClosedXML.Excel;
using TalentoPlus.Application.DTOs.Common;
using TalentoPlus.Application.Services.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class ExcelService : IExcelService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IJobPositionRepository _jobPositionRepository;

    public ExcelService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IJobPositionRepository jobPositionRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _jobPositionRepository = jobPositionRepository;
    }

    public async Task<ImportResultDto> ImportEmployeesAsync(Stream fileStream)
    {
        var result = new ImportResultDto();
        var departments = await _departmentRepository.GetAllAsync();
        var jobPositions = await _jobPositionRepository.GetAllAsync();

        try
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed();
            
            if (!rows.Any())
            {
                result.Errors.Add("El archivo está vacío.");
                return result;
            }

            // 1. Buscar la fila de encabezados (puede no ser la primera)
            IXLRangeRow headerRow = null;
            foreach (var row in rows.Take(10)) // Buscar en las primeras 10 filas
            {
                var cells = row.CellsUsed();
                if (cells.Any(c => IsMatch(c.GetValue<string>().Trim().ToLower(), "email", "correo", "mail")))
                {
                    headerRow = row;
                    break;
                }
            }

            if (headerRow == null)
            {
                // Fallback: usar la primera fila si no se encuentra "Email"
                headerRow = rows.First();
            }

            var map = MapColumns(headerRow);
            
            // Validar columna crítica
            if (!map.ContainsKey("Email")) 
            {
                result.Errors.Add("No se encontró una columna de 'Email' o 'Correo' en la cabecera.");
                return result;
            }

            // 2. Procesar filas de datos (empezar después de la cabecera)
            var dataRows = rows.Where(r => r.RowNumber() > headerRow.RowNumber());

            foreach (var row in dataRows)
            {
                result.TotalProcessed++;
                try
                {
                    // Función local para obtener valor seguro
                    string GetVal(string key) => map.ContainsKey(key) 
                        ? row.Cell(map[key]).GetValue<string>().Trim() 
                        : string.Empty;

                    var email = GetVal("Email");
                    var docNumber = GetVal("DocumentNumber");

                    // Validaciones básicas
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        // Si la fila está vacía visualmente, saltarla sin error
                        if (row.IsEmpty()) continue;

                        result.FailedImports++;
                        result.Errors.Add($"Fila {row.RowNumber()}: Email vacío.");
                        continue;
                    }

                    // Validar duplicados (si tenemos documento)
                    if (!string.IsNullOrEmpty(docNumber) && await _employeeRepository.DocumentNumberExistsAsync(docNumber))
                    {
                        result.FailedImports++;
                        result.Errors.Add($"Fila {row.RowNumber()}: Documento {docNumber} ya existe.");
                        continue;
                    }

                    if (await _employeeRepository.EmailExistsAsync(email))
                    {
                        result.FailedImports++;
                        result.Errors.Add($"Fila {row.RowNumber()}: Email {email} ya existe.");
                        continue;
                    }

                    // Buscar o Crear Departamento
                    var deptName = GetVal("Department");
                    if (string.IsNullOrWhiteSpace(deptName)) deptName = "General";

                    var department = departments.FirstOrDefault(d => d.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase));
                    if (department == null)
                    {
                        // Generar código único: 3 primeras letras mayúsculas + random
                        var codePrefix = deptName.Length >= 3 ? deptName.Substring(0, 3).ToUpper() : deptName.ToUpper();
                        var deptCode = $"{codePrefix}-{new Random().Next(100, 999)}";

                        department = new Department 
                        { 
                            Name = deptName, 
                            Code = deptCode, // Asignar código generado
                            Description = "Creado automáticamente por importación",
                            IsActive = true 
                        };
                        await _departmentRepository.AddAsync(department);
                        await _departmentRepository.SaveChangesAsync();
                        departments = (await _departmentRepository.GetAllAsync()).ToList();
                    }

                    // Buscar o Crear Cargo
                    var jobTitle = GetVal("JobPosition");
                    if (string.IsNullOrWhiteSpace(jobTitle)) jobTitle = "Empleado General";

                    var jobPosition = jobPositions.FirstOrDefault(j => j.Title.Equals(jobTitle, StringComparison.OrdinalIgnoreCase) && j.DepartmentId == department.Id);
                    if (jobPosition == null)
                    {
                        jobPosition = new JobPosition 
                        { 
                            Title = jobTitle, 
                            Description = "Creado automáticamente por importación",
                            DepartmentId = department.Id,
                            IsActive = true,
                            MinSalary = 0,
                            MaxSalary = 0
                        };
                        await _jobPositionRepository.AddAsync(jobPosition);
                        await _jobPositionRepository.SaveChangesAsync();
                        jobPositions = (await _jobPositionRepository.GetAllAsync()).ToList();
                    }

                    // Crear empleado
                    var employee = new Employee
                    {
                        DocumentType = GetVal("DocumentType") == "" ? "CC" : GetVal("DocumentType"),
                        DocumentNumber = string.IsNullOrEmpty(docNumber) ? "SIN-DOC-" + Guid.NewGuid().ToString().Substring(0,8) : docNumber,
                        FirstName = GetVal("FirstName") == "" ? "Sin Nombre" : GetVal("FirstName"),
                        LastName = GetVal("LastName") == "" ? "Sin Apellido" : GetVal("LastName"),
                        PersonalEmail = email,
                        DepartmentId = department.Id,
                        JobPositionId = jobPosition.Id,
                        Salary = TryParseDecimal(GetVal("Salary")),
                        HireDate = TryParseDate(GetVal("HireDate")),
                        
                        IsActive = true,
                        Status = EmployeeStatus.Activo,
                        Country = "Colombia",
                        PhoneNumber = GetVal("PhoneNumber") == "" ? "0000000000" : GetVal("PhoneNumber"),
                        DateOfBirth = DateTime.Today.AddYears(-20)
                    };

                    await _employeeRepository.AddAsync(employee);
                    await _employeeRepository.SaveChangesAsync();
                    result.SuccessfulImports++;
                }
                catch (Exception ex)
                {
                    // CRÍTICO: Limpiar el contexto para que el error no contamine la siguiente iteración
                    _employeeRepository.ClearChangeTracker();

                    result.FailedImports++;
                    var msg = ex.InnerException?.Message ?? ex.Message;
                    result.Errors.Add($"Fila {row.RowNumber()}: Error - {msg}");
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Error crítico leyendo el archivo: {ex.Message}");
        }

        return result;
    }

    // Método auxiliar para detectar columnas por nombres comunes
    private Dictionary<string, int> MapColumns(IXLRangeRow headerRow)
    {
        var mapping = new Dictionary<string, int>();
        var cells = headerRow.CellsUsed();

        foreach (var cell in cells)
        {
            var header = cell.GetValue<string>().Trim().ToLower();
            var colIndex = cell.Address.ColumnNumber;

            // Orden optimizado y validaciones más estrictas para evitar confusiones
            if (IsMatch(header, "nombre", "nombres", "first name")) mapping["FirstName"] = colIndex;
            else if (IsMatch(header, "apellido", "apellidos", "last name")) mapping["LastName"] = colIndex;
            else if (IsMatch(header, "tipo", "doc type")) mapping["DocumentType"] = colIndex;
            // "id" solo si es palabra exacta o está acompañada de "doc" o "num"
            else if (IsMatch(header, "número", "numero", "num", "cédula", "cedula", "documento", "document number") || header == "id") mapping["DocumentNumber"] = colIndex;
            else if (IsMatch(header, "email", "correo", "mail")) mapping["Email"] = colIndex;
            else if (IsMatch(header, "departamento", "department", "área", "area")) mapping["Department"] = colIndex;
            else if (IsMatch(header, "cargo", "puesto", "job", "rol", "position")) mapping["JobPosition"] = colIndex;
            else if (IsMatch(header, "salario", "sueldo", "salary")) mapping["Salary"] = colIndex;
            else if (IsMatch(header, "ingreso", "inicio", "hire")) mapping["HireDate"] = colIndex;
            else if (IsMatch(header, "teléfono", "telefono", "celular", "phone")) mapping["PhoneNumber"] = colIndex;
        }

        return mapping;
    }

    private bool IsMatch(string header, params string[] keywords)
    {
        return keywords.Any(k => header.Contains(k));
    }

    private decimal TryParseDecimal(string value)
    {
        if (decimal.TryParse(value, out var result)) return result;
        return 0;
    }

    private DateTime TryParseDate(string value)
    {
        if (DateTime.TryParse(value, out var result)) return result;
        return DateTime.Today;
    }
}
