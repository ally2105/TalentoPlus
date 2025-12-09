using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Application.Services.Interfaces;
using TalentoPlus.Application.Services.Implementations;
using FluentValidation;
using TalentoPlus.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null
        )
    )
);

// Registrar repositorios
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IJobPositionRepository, JobPositionRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registrar servicios de aplicación
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IExcelService, TalentoPlus.Infrastructure.Services.ExcelService>();
builder.Services.AddScoped<IPdfService, TalentoPlus.Infrastructure.Services.PdfService>();

// Registrar validadores
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeCreateDtoValidator>();

// Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;

    // Configuración de bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Sembrar datos iniciales (roles y usuario admin)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        await DbSeeder.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error al sembrar datos iniciales");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
