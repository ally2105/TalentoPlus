using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Application.DTOs.Auth;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Data;
using Xunit;

namespace TalentoPlus.Tests.Integration.Api;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Configurar servicios de prueba si es necesario (ej: base de datos en memoria)
            // Por ahora usaremos la configuración por defecto, pero idealmente se debe reemplazar la DB
        });
        _client = _factory.CreateClient();
    }

    // Nota: Esta prueba requiere una base de datos real o configurada en memoria en el factory.
    // Como configurar InMemory para toda la API es complejo en este paso sin alterar mucho,
    // haremos una prueba simple que verifique que el endpoint responde (aunque sea 401 o 400).
    
    [Fact]
    public async Task Login_Should_Return_BadRequest_When_Body_Is_Invalid()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/empleados/login", new LoginRequestDto());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Credentials_Are_Wrong()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/empleados/login", new LoginRequestDto 
        { 
            Email = "wrong@email.com", 
            Password = "wrongpassword" 
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
