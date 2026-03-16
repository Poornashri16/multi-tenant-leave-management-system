// --------------------
// ALL using statements at the top
// --------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

using SaaSPlatform.Application;  // IProfileService implementation
using SaaSPlatform.Infrastructure;
using SaaSPlatform.Persistence.Context;
using SaaSPlatform.API.Services;           // UserService, LeaveService
using SaaSPlatform.API.Services.Interfaces;
using SaaSPlatform.Application.Services; // <-- Add this

// --------------------
// Create builder
// --------------------
var builder = WebApplication.CreateBuilder(args);

// --------------------
// Add DbContext
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------
// Add Application & Infrastructure Services
// --------------------
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// --------------------
// Register Custom Services
// --------------------
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ReimbursementService>();


// --------------------
// Add Controllers
// --------------------
builder.Services.AddControllers();

// --------------------
// Enable CORS
// --------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --------------------
// JWT Authentication
// --------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        ),

        RoleClaimType = ClaimTypes.Role
    };
});

// --------------------
// Authorization
// --------------------
builder.Services.AddAuthorization();

// --------------------
// Swagger / OpenAPI
// --------------------
builder.Services.AddEndpointsApiExplorer();

// --------------------
// Build App
// --------------------
var app = builder.Build();
// --- TEMPORARY TEST START ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Console.WriteLine("EF is connected to database: " + context.Database.GetDbConnection().Database);

    // Optional: list all table names
    var tables = context.Database.ExecuteSqlRaw(
        "SELECT table_name FROM information_schema.tables WHERE table_schema='public';");
}
// --- TEMPORARY TEST END ---

// --------------------
// Serve Frontend Files
// --------------------
app.UseDefaultFiles();
app.UseStaticFiles();

// --------------------
// Middleware
// --------------------
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// --------------------
// Map Controllers
// --------------------
app.MapControllers();

// --------------------
// Run
// --------------------
app.Run();