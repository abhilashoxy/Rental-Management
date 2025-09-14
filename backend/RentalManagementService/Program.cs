using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalManagementService.Data;
using RentalManagementService.Interfaces;
using RentalManagementService.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1) Configuration (includes environment variables)
builder.Configuration.AddEnvironmentVariables();

// 2) MVC + JSON options (ignore cycles)
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    // o.JsonSerializerOptions.MaxDepth = 64; // if you need deeper graphs
});

// 3) Database (MySQL) — reads ConnectionStrings:Default or env var ConnectionStrings__Default
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration["ConnectionStrings:Default"]
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
        ?? throw new InvalidOperationException("Missing connection string: set ConnectionStrings:Default / ConnectionStrings__Default.");

    opt.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// 4) CORS — allow Angular dev or your Nginx origin
var frontendOrigin = builder.Configuration["FRONTEND_ORIGIN"] ?? "http://localhost:4200";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
                frontendOrigin.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

// 5) JWT auth
var jwtKey = builder.Configuration["Jwt:Key"] ?? builder.Configuration["JWT_KEY"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? builder.Configuration["JWT_ISSUER"] ?? "rental-mgmt";
var jwtAud = builder.Configuration["Jwt:Audience"] ?? builder.Configuration["JWT_AUDIENCE"] ?? "rental-mgmt-clients";

// Fail fast with a clear message if key is missing/too short
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException("JWT signing key missing/weak. Provide Jwt:Key (or env JWT_KEY) with at least 16 chars.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // dev only
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAud,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// 6) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 7) Email sender — Ethereal SMTP in dev, SendGrid in prod (adjust as you like)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();     // Ethereal via SMTP
}
else
{
    builder.Services.AddScoped<IEmailSender, SendGridEmailSender>(); // or use SMTP in prod too
}

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
    // If you want to lock to Docker network IP of nginx, add KnownProxies/KnownNetworks
    // KnownProxies = { IPAddress.Parse("172.19.0.2") }
});

// 8) Middleware
app.UseCors("AllowFrontend");
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 9) Health endpoint (for Docker/K8s checks)
app.MapGet("/health", () => Results.Ok("ok"));

// 10) DB migrate & seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync(); // better than EnsureCreated for relational DBs
    DbSeeder.Seed(db);
}

app.Run();
