using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Application.Services.Interfaces;

namespace TalentoPlus.Infrastructure.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        // Configurar licencia Community
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateEmployeeResume(EmployeeDto employee)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(header => ComposeHeader(header, employee));
                page.Content().Element(content => ComposeContent(content, employee));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generado por TalentoPlus - ");
                    x.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, EmployeeDto employee)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(employee.FullName).FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                column.Item().Text(employee.JobPositionTitle).FontSize(14).FontColor(Colors.Grey.Darken2);
                column.Item().Text(employee.DepartmentName).FontSize(12).FontColor(Colors.Grey.Darken1);
            });

            row.ConstantItem(100).Height(100).Background(Colors.Grey.Lighten3).AlignCenter().AlignMiddle().Text(employee.FullName.Substring(0, 1)).FontSize(40).FontColor(Colors.Grey.Darken2);
        });
    }

    private void ComposeContent(IContainer container, EmployeeDto employee)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(15);

            // Información Personal
            column.Item().Element(c => Section(c, "Información Personal"));
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(120);
                    columns.RelativeColumn();
                });

                TableRow(table, "Documento:", $"{employee.DocumentType} {employee.DocumentNumber}");
                TableRow(table, "Fecha Nacimiento:", $"{employee.DateOfBirth:dd/MM/yyyy} ({employee.Age} años)");
                TableRow(table, "Género:", employee.Gender ?? "N/A");
                TableRow(table, "Estado Civil:", "N/A"); // Placeholder
            });

            // Información de Contacto
            column.Item().Element(c => Section(c, "Contacto"));
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(120);
                    columns.RelativeColumn();
                });

                TableRow(table, "Email Personal:", employee.PersonalEmail);
                TableRow(table, "Email Corporativo:", employee.CorporateEmail ?? "N/A");
                TableRow(table, "Teléfono:", employee.PhoneNumber);
                TableRow(table, "Dirección:", $"{employee.Address}, {employee.City}, {employee.Country}");
            });

            // Información Laboral
            column.Item().Element(c => Section(c, "Información Laboral"));
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(120);
                    columns.RelativeColumn();
                });

                TableRow(table, "Fecha Ingreso:", employee.HireDate.ToString("dd/MM/yyyy"));
                TableRow(table, "Antigüedad:", $"{employee.YearsOfService:F1} años");
                TableRow(table, "Salario:", $"${employee.Salary:N0}");
                TableRow(table, "Estado:", employee.StatusName);
            });

            // Perfil Profesional
            if (!string.IsNullOrEmpty(employee.ProfessionalProfile))
            {
                column.Item().Element(c => Section(c, "Perfil Profesional"));
                column.Item().Text(employee.ProfessionalProfile).Justify();
            }
        });
    }

    private void Section(IContainer container, string title)
    {
        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text(title).FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
    }

    private void TableRow(TableDescriptor table, string label, string value)
    {
        table.Cell().Text(label).SemiBold().FontColor(Colors.Grey.Darken2);
        table.Cell().Text(value);
    }
}
