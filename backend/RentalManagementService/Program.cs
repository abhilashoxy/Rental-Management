using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalManagementService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(opt => {
  var cs = builder.Configuration.GetConnectionString("Default")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
    ?? throw new Exception("Missing connection string");
  opt.UseMySql(cs, ServerVersion.AutoDetect(cs));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opt => {
    var cfg = builder.Configuration.GetSection("Jwt");
    var key = cfg["Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key") ?? "dev-12345678901234567890123456789012";
    opt.TokenValidationParameters = new TokenValidationParameters{
      ValidateIssuer=true, ValidateAudience=true, ValidateIssuerSigningKey=true,
      ValidIssuer = cfg["Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer"),
      ValidAudience = cfg["Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience"),
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
  });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();
app.UseAuthentication(); app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope()) {
  var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  db.Database.EnsureCreated();
  DbSeeder.Seed(db);
}
app.Run();
