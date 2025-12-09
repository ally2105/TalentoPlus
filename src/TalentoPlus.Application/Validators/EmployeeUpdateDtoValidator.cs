using FluentValidation;
using TalentoPlus.Application.DTOs.Employees;

namespace TalentoPlus.Application.Validators;

public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID del empleado es inválido");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento es requerido")
            .MaximumLength(50).WithMessage("El número de documento no puede exceder 50 caracteres");

        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("El tipo de documento es requerido");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El primer nombre es requerido")
            .MaximumLength(100).WithMessage("El primer nombre no puede exceder 100 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El primer apellido es requerido")
            .MaximumLength(100).WithMessage("El primer apellido no puede exceder 100 caracteres");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("La fecha de nacimiento es requerida")
            .LessThan(DateTime.Today.AddYears(-18)).WithMessage("El empleado debe ser mayor de 18 años");

        RuleFor(x => x.PersonalEmail)
            .NotEmpty().WithMessage("El correo personal es requerido")
            .EmailAddress().WithMessage("El formato del correo personal no es válido");

        RuleFor(x => x.CorporateEmail)
            .EmailAddress().WithMessage("El formato del correo corporativo no es válido")
            .When(x => !string.IsNullOrEmpty(x.CorporateEmail));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El número de teléfono es requerido");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("La fecha de contratación es requerida");

        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("El salario debe ser mayor a 0");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Debe seleccionar un departamento");

        RuleFor(x => x.JobPositionId)
            .GreaterThan(0).WithMessage("Debe seleccionar un cargo");
    }
}
