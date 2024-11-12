using Microsoft.EntityFrameworkCore;
using CFAGestionClientes.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddDbContext<CFAContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 30))));

// Configurar JWT Authentication
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(key) || key.Length < 32)
{
    throw new ArgumentNullException("La clave JWT debe tener al menos 32 caracteres.");
}

var keyBytes = Encoding.UTF8.GetBytes(key); // Cambia a Encoding.UTF8 para mayor compatibilidad
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Issuer no puede ser nulo."),
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Audience no puede ser nulo."),
        ClockSkew = TimeSpan.Zero // Reduce el tiempo de tolerancia para la expiración del token
    };
});

var app = builder.Build();

// Configurar el middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Asegúrate de que esto esté antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();