using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Application.Services.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmailService _emailService;

    public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IEmailService emailService)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _emailService = emailService;
    }

    /// <summary>
    /// Retrieves all employees registered in the system.
    /// </summary>
    public async Task<IEnumerable<EmployeeListDto>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        
        return employees.Select(e => new EmployeeListDto
        {
            Id = e.Id,
            FullName = e.FullName,
            DocumentNumber = e.DocumentNumber,
            PersonalEmail = e.PersonalEmail,
            CorporateEmail = e.CorporateEmail,
            DepartmentName = e.Department?.Name ?? "Sin Departamento",
            JobPositionTitle = e.JobPosition?.Title ?? "Sin Cargo",
            Status = e.Status,
            HireDate = e.HireDate
        });
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(id);
        if (employee == null) return null;

        return new EmployeeDto
        {
            Id = employee.Id,
            DocumentNumber = employee.DocumentNumber,
            DocumentType = employee.DocumentType,
            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            LastName = employee.LastName,
            SecondLastName = employee.SecondLastName,
            FullName = employee.FullName,
            DateOfBirth = employee.DateOfBirth,
            Age = employee.Age,
            Gender = employee.Gender,
            PersonalEmail = employee.PersonalEmail,
            CorporateEmail = employee.CorporateEmail,
            PhoneNumber = employee.PhoneNumber,
            AlternativePhoneNumber = employee.AlternativePhoneNumber,
            Address = employee.Address,
            City = employee.City,
            Country = employee.Country,
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            Salary = employee.Salary,
            Status = employee.Status,
            StatusName = employee.Status.ToString(),
            ProfessionalProfile = employee.ProfessionalProfile,
            YearsOfService = employee.YearsOfService,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? string.Empty,
            JobPositionId = employee.JobPositionId,
            JobPositionTitle = employee.JobPosition?.Title ?? string.Empty,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            IsActive = employee.IsActive
        };
    }

    /// <summary>
    /// Creates a new employee validating document and email duplicates.
    /// </summary>
    public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
    {
        if (await _employeeRepository.DocumentNumberExistsAsync(dto.DocumentNumber))
            throw new InvalidOperationException($"Ya existe un empleado con el documento {dto.DocumentNumber}");

        if (await _employeeRepository.EmailExistsAsync(dto.PersonalEmail))
            throw new InvalidOperationException($"Ya existe un empleado con el email {dto.PersonalEmail}");

        var employee = new Employee
        {
            DocumentNumber = dto.DocumentNumber,
            DocumentType = dto.DocumentType,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            SecondLastName = dto.SecondLastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            PersonalEmail = dto.PersonalEmail,
            CorporateEmail = dto.CorporateEmail,
            PhoneNumber = dto.PhoneNumber,
            AlternativePhoneNumber = dto.AlternativePhoneNumber,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            HireDate = dto.HireDate,
            Salary = dto.Salary,
            Status = dto.Status,
            ProfessionalProfile = dto.ProfessionalProfile,
            DepartmentId = dto.DepartmentId,
            JobPositionId = dto.JobPositionId,
            IsActive = true
        };

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveChangesAsync(); // Guardar cambios

        // Retornar DTO con el ID generado
        return await GetByIdAsync(employee.Id) ?? throw new InvalidOperationException("Error al crear empleado");
    }

    /// <summary>
    /// Updates an existing employee's information.
    /// </summary>
    public async Task UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        if (id != dto.Id)
            throw new ArgumentException("El ID no coincide");

        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"No se encontró el empleado con ID {id}");

        if (await _employeeRepository.DocumentNumberExistsAsync(dto.DocumentNumber, id))
            throw new InvalidOperationException($"Ya existe otro empleado con el documento {dto.DocumentNumber}");

        if (await _employeeRepository.EmailExistsAsync(dto.PersonalEmail, id))
            throw new InvalidOperationException($"Ya existe otro empleado con el email {dto.PersonalEmail}");

        // Actualizar campos
        employee.DocumentNumber = dto.DocumentNumber;
        employee.DocumentType = dto.DocumentType;
        employee.FirstName = dto.FirstName;
        employee.MiddleName = dto.MiddleName;
        employee.LastName = dto.LastName;
        employee.SecondLastName = dto.SecondLastName;
        employee.DateOfBirth = dto.DateOfBirth;
        employee.Gender = dto.Gender;
        employee.PersonalEmail = dto.PersonalEmail;
        employee.CorporateEmail = dto.CorporateEmail;
        employee.PhoneNumber = dto.PhoneNumber;
        employee.AlternativePhoneNumber = dto.AlternativePhoneNumber;
        employee.Address = dto.Address;
        employee.City = dto.City;
        employee.Country = dto.Country;
        employee.HireDate = dto.HireDate;
        employee.TerminationDate = dto.TerminationDate;
        employee.Salary = dto.Salary;
        employee.Status = dto.Status;
        employee.ProfessionalProfile = dto.ProfessionalProfile;
        employee.DepartmentId = dto.DepartmentId;
        employee.JobPositionId = dto.JobPositionId;
        employee.JobPositionId = dto.JobPositionId;

        await _employeeRepository.UpdateAsync(employee);
        await _employeeRepository.SaveChangesAsync(); // Guardar cambios
    }

    /// <summary>
    /// Performs a logical deletion (Soft Delete) of the employee.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee != null)
        {
            employee.IsActive = false;
            employee.Status = Domain.Enums.EmployeeStatus.Retirado;
            employee.TerminationDate = DateTime.UtcNow;
            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync(); // Guardar cambios
            Console.WriteLine($"✅ Empleado {id} eliminado (Soft Delete) y cambios guardados.");
        }
    }

    public async Task<IEnumerable<EmployeeListDto>> SearchAsync(string searchTerm)
    {
        var employees = await _employeeRepository.SearchAsync(searchTerm);
        
        return employees.Select(e => new EmployeeListDto
        {
            Id = e.Id,
            FullName = e.FullName,
            DocumentNumber = e.DocumentNumber,
            PersonalEmail = e.PersonalEmail,
            CorporateEmail = e.CorporateEmail,
            DepartmentName = e.Department?.Name ?? "Sin Departamento",
            JobPositionTitle = e.JobPosition?.Title ?? "Sin Cargo",
            Status = e.Status,
            HireDate = e.HireDate
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        return employee != null;
    }

    public async Task DeleteAllAsync()
    {
        await _employeeRepository.DeleteAllAsync();
    }

    /// <summary>
    /// Calculates key statistics for the administrative Dashboard.
    /// </summary>
    public async Task<TalentoPlus.Application.DTOs.Dashboard.DashboardStatsDto> GetDashboardStatsAsync()
    {
        var stats = new TalentoPlus.Application.DTOs.Dashboard.DashboardStatsDto();
        var countByDept = await _employeeRepository.GetEmployeeCountByDepartmentAsync();
        var countByStatus = await _employeeRepository.GetEmployeeCountByStatusAsync();
        
        // Obtener departamentos para nombres
        var departments = await _departmentRepository.GetAllAsync();
        var deptMap = departments.ToDictionary(d => d.Id, d => d.Name);

        // Mapear departamentos
        foreach (var kvp in countByDept)
        {
            var deptName = deptMap.ContainsKey(kvp.Key) ? deptMap[kvp.Key] : "Sin Departamento";
            stats.EmployeesByDepartment[deptName] = kvp.Value;
        }

        // Mapear estados
        foreach (var kvp in countByStatus)
        {
            stats.EmployeesByStatus[kvp.Key.ToString()] = kvp.Value;
        }

        // Totales
        stats.TotalEmployees = countByStatus.Values.Sum();
        stats.ActiveEmployees = countByStatus.ContainsKey(Domain.Enums.EmployeeStatus.Activo) ? countByStatus[Domain.Enums.EmployeeStatus.Activo] : 0;
        stats.EmployeesOnVacation = countByStatus.ContainsKey(Domain.Enums.EmployeeStatus.Vacaciones) ? countByStatus[Domain.Enums.EmployeeStatus.Vacaciones] : 0;
        
        // Inactivos son todos los que no están Activos ni en Vacaciones
        stats.InactiveEmployees = stats.TotalEmployees - stats.ActiveEmployees - stats.EmployeesOnVacation;

        return stats;
    }

    /// <summary>
    /// Registers an employee from the public API and sends a welcome email.
    /// </summary>
    public async Task RegisterAsync(TalentoPlus.Application.DTOs.Auth.EmployeeRegisterRequest request)
    {
        if (await _employeeRepository.DocumentNumberExistsAsync(request.DocumentNumber))
            throw new InvalidOperationException($"Ya existe un empleado con el documento {request.DocumentNumber}");

        if (await _employeeRepository.EmailExistsAsync(request.Email))
            throw new InvalidOperationException($"Ya existe un empleado con el email {request.Email}");

        // Crear entidad
        var employee = new Employee
        {
            DocumentNumber = request.DocumentNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PersonalEmail = request.Email,
            DepartmentId = request.DepartmentId,
            JobPositionId = request.JobPositionId,
            HireDate = DateTime.UtcNow,
            IsActive = true,
            Status = Domain.Enums.EmployeeStatus.Activo,
            PasswordHash = HashPassword(request.Password) // Hash simple
        };

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveChangesAsync();

        // Enviar Email
        try 
        {
            var subject = "Bienvenido a TalentoPlus - Registro Exitoso";
            var body = $@"
                <h2>Hola {employee.FirstName},</h2>
                <p>Tu registro en <strong>TalentoPlus</strong> ha sido exitoso.</p>
                <p>Tus datos han sido recibidos correctamente. Podrás autenticarte en la plataforma una vez que tu cuenta sea habilitada por el administrador.</p>
                <br>
                <p>Atentamente,<br>El equipo de TalentoPlus</p>";

            await _emailService.SendEmailAsync(employee.PersonalEmail, subject, body);
        }
        catch 
        {
            // Log error pero no fallar el registro
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
