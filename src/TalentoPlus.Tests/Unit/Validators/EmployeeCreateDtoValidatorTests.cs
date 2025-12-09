using FluentValidation.TestHelper;
using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Application.Validators;
using Xunit;

namespace TalentoPlus.Tests.Unit.Validators;

public class EmployeeCreateDtoValidatorTests
{
    private readonly EmployeeCreateDtoValidator _validator;

    public EmployeeCreateDtoValidatorTests()
    {
        _validator = new EmployeeCreateDtoValidator();
    }

    [Fact]
    public void Should_Have_Error_When_DocumentNumber_Is_Empty()
    {
        var model = new EmployeeCreateDto { DocumentNumber = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new EmployeeCreateDto { PersonalEmail = "invalid-email" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PersonalEmail);
    }

    [Fact]
    public void Should_Have_Error_When_Salary_Is_Negative()
    {
        var model = new EmployeeCreateDto { Salary = -100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new EmployeeCreateDto
        {
            DocumentNumber = "123456789",
            FirstName = "John",
            LastName = "Doe",
            PersonalEmail = "john.doe@example.com",
            Salary = 1000,
            DepartmentId = 1,
            JobPositionId = 1,
            HireDate = DateTime.Now
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DocumentNumber);
        result.ShouldNotHaveValidationErrorFor(x => x.PersonalEmail);
        result.ShouldNotHaveValidationErrorFor(x => x.Salary);
    }
}
