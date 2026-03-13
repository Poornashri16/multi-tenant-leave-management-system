using SaaSPlatform.Application;
using SaaSPlatform.Infrastructure;
using SaaSPlatform.Persistence.Context;
using SaaSPlatform.API.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using System.Security.Claims;

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

// --------------------
// Add Controllers
// --------------------
builder.Services.AddControllers();

// --------------------
// Enable CORS (for frontend JS to call API)
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

        // VERY IMPORTANT: Correct role claim mapping
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

// --------------------
// Serve Frontend Files
// --------------------
app.UseDefaultFiles();  
app.UseStaticFiles();   

// --------------------
// Middleware Pipeline
// --------------------
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();   // must come before Authorization
app.UseAuthorization();

// --------------------
// Map Controllers
// --------------------
app.MapControllers();

// --------------------
// Run Application
// --------------------
app.Run();