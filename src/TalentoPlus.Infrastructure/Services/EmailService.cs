using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

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
        try
        {
            var emailMessage = new MimeMessage();
            
            var fromAddress = _configuration["Smtp:FromAddress"] ?? "noreply@talentoplus.com";
            var fromName = _configuration["Smtp:FromName"] ?? "TalentoPlus";
            
            emailMessage.From.Add(new MailboxAddress(fromName, fromAddress));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            // Configuración del servidor
            var server = _configuration["Smtp:Server"];
            var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];
            var useSsl = bool.Parse(_configuration["Smtp:UseSsl"] ?? "true");

            if (string.IsNullOrEmpty(server))
            {
                _logger.LogWarning("⚠️ Configuración SMTP no encontrada. El correo NO se envió.");
                return;
            }

            // Conectar
            // StartTls es lo más común para puerto 587. SslOnConnect para 465.
            var socketOptions = useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
            if (port == 465) socketOptions = SecureSocketOptions.SslOnConnect;

            await client.ConnectAsync(server, port, socketOptions);

            // Autenticar
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }

            // Enviar
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"✅ Correo enviado exitosamente a {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ Error enviando correo a {to}: {ex.Message}");
            throw; // Re-lanzar para que quien llame sepa que falló, o manejar según política
        }
    }
}
