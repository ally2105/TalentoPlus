using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Application.Services.Interfaces;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IJobPositionRepository _jobPositionRepository;
    private readonly IExcelService _excelService;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly IValidator<EmployeeCreateDto> _createValidator;
    private readonly IValidator<EmployeeUpdateDto> _updateValidator;

    public EmployeesController(
        IEmployeeService employeeService,
        IDepartmentRepository departmentRepository,
        IJobPositionRepository jobPositionRepository,
        IExcelService excelService,
        IPdfService pdfService,
        IEmailService emailService,
        IValidator<EmployeeCreateDto> createValidator,
        IValidator<EmployeeUpdateDto> updateValidator)
    {
        _employeeService = employeeService;
        _departmentRepository = departmentRepository;
        _jobPositionRepository = jobPositionRepository;
        _excelService = excelService;
        _pdfService = pdfService;
        _emailService = emailService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // GET: Employees
    public async Task<IActionResult> Index(string searchTerm)
    {
        IEnumerable<EmployeeListDto> employees;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            employees = await _employeeService.SearchAsync(searchTerm);
            ViewData["CurrentFilter"] = searchTerm;
        }
        else
        {
            employees = await _employeeService.GetAllAsync();
        }

        return View(employees);
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // GET: Employees/Create
    public async Task<IActionResult> Create()
    {
        await LoadViewBags();
        return View(new EmployeeCreateDto());
    }

    // POST: Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeCreateDto employee)
    {
        var validationResult = await _createValidator.ValidateAsync(employee);
        
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _employeeService.CreateAsync(employee);
                TempData["SuccessMessage"] = "Empleado creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        await LoadViewBags();
        return View(employee);
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var updateDto = new EmployeeUpdateDto
        {
            Id = employee.Id,
            DocumentNumber = employee.DocumentNumber,
            DocumentType = employee.DocumentType,
            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            LastName = employee.LastName,
            SecondLastName = employee.SecondLastName,
            DateOfBirth = employee.DateOfBirth,
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
            ProfessionalProfile = employee.ProfessionalProfile,
            DepartmentId = employee.DepartmentId,
            JobPositionId = employee.JobPositionId,
            IsActive = employee.IsActive
        };

        await LoadViewBags();
        return View(updateDto);
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeUpdateDto employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        var validationResult = await _updateValidator.ValidateAsync(employee);
        
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _employeeService.UpdateAsync(id, employee);
                TempData["SuccessMessage"] = "Empleado actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        await LoadViewBags();
        return View(employee);
    }

    // GET: Employees/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _employeeService.DeleteAsync(id);
        TempData["SuccessMessage"] = "Empleado eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Employees/Import
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Por favor seleccione un archivo Excel válido.";
            return RedirectToAction(nameof(Index));
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "Solo se permiten archivos con extensión .xlsx";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _excelService.ImportEmployeesAsync(stream);

            if (result.FailedImports == 0)
            {
                TempData["SuccessMessage"] = $"Importación exitosa. Se procesaron {result.SuccessfulImports} empleados.";
            }
            else
            {
                var errorMsg = $"Procesados: {result.TotalProcessed}. Exitosos: {result.SuccessfulImports}. Fallidos: {result.FailedImports}. <br/>";
                if (result.Errors.Any())
                {
                    errorMsg += "Errores: " + string.Join(", ", result.Errors.Take(3));
                    if (result.Errors.Count > 3) errorMsg += "...";
                }
                TempData["WarningMessage"] = errorMsg;
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al procesar el archivo: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Employees/DeleteAll
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAll()
    {
        try
        {
            await _employeeService.DeleteAllAsync();
            TempData["SuccessMessage"] = "Se han eliminado TODOS los registros (Empleados, Cargos y Departamentos). Base de datos limpia.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al eliminar registros: " + ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Employees/DownloadResume/5
    public async Task<IActionResult> DownloadResume(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        try
        {
            var pdfBytes = _pdfService.GenerateEmployeeResume(employee);
            return File(pdfBytes, "application/pdf", $"HV_{employee.DocumentNumber}_{employee.LastName}.pdf");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al generar el PDF: " + ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    // POST: Employees/SendEmail/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendEmail(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            TempData["ErrorMessage"] = "Empleado no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrEmpty(employee.PersonalEmail))
        {
            TempData["ErrorMessage"] = "El empleado no tiene un correo electrónico registrado.";
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            var subject = "Bienvenido a TalentoPlus - Registro Exitoso";
            var body = $@"
                <h2>Hola {employee.FirstName},</h2>
                <p>Tu registro en <strong>TalentoPlus</strong> ha sido exitoso.</p>
                <p>Tus datos han sido recibidos correctamente. Podrás autenticarte en la plataforma una vez que tu cuenta sea habilitada por el administrador.</p>
                <br>
                <p><strong>Información de tu cuenta:</strong></p>
                <ul>
                    <li>Documento: {employee.DocumentNumber}</li>
                    <li>Departamento: {employee.DepartmentName}</li>
                    <li>Cargo: {employee.JobPositionTitle}</li>
                </ul>
                <br>
                <p>Atentamente,<br>El equipo de TalentoPlus</p>";

            await _emailService.SendEmailAsync(employee.PersonalEmail, subject, body);

            TempData["SuccessMessage"] = $"✅ Correo enviado exitosamente a {employee.PersonalEmail}";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al enviar el correo: {ex.Message}";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadViewBags()
    {
        var departments = await _departmentRepository.GetAllAsync();
        ViewData["Departments"] = new SelectList(departments, "Id", "Name");
        
        var jobPositions = await _jobPositionRepository.GetAllAsync();
        ViewData["JobPositions"] = new SelectList(jobPositions, "Id", "Title");
    }
}
