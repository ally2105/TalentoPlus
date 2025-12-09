using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TalentoPlus.Infrastructure.Services;

public class EmailService : TalentoPlus.Application.Services.Interfaces.IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Aquí iría la implementación real con MailKit o SendGrid
        // var smtpServer = _configuration["Smtp:Server"];
        // ...
        
        // Simulación para Demo
        _logger.LogInformation($"📧 [EMAIL SIMULADO] Enviando a: {to}");
        _logger.LogInformation($"   Asunto: {subject}");
        _logger.LogInformation($"   Cuerpo: {body}");
        
        await Task.CompletedTask;
    }
}
