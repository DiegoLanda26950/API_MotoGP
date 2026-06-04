using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MotoGP_API.Configurations;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Cloudinary desde appsettings.json
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);

// Configuración de JWT
builder.Services.AddAuthentication(options =>
{
    // Esquema de autenticación por defecto
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validamos el emisor del token
        ValidateIssuer = true,
        // Validamos la audiencia del token
        ValidateAudience = true,
        // Validamos la fecha de expiración
        ValidateLifetime = true,
        // Validamos la firma del token
        ValidateIssuerSigningKey = true,
        // Valores válidos del emisor y audiencia
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        // Clave secreta para firmar el token
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
    };
});

// Registro de repositorios
builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<ICircuitoRepository, CircuitoRepository>();
builder.Services.AddScoped<IPilotoRepository, PilotoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Registro de servicios
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IMotoService, MotoService>();
builder.Services.AddScoped<ICircuitoService, CircuitoService>();
builder.Services.AddScoped<IPilotoService, PilotoService>();

// Registro del servicio de subida de imágenes
builder.Services.AddScoped<IUploadService, CloudinaryUploadService>();

// Registro del servicio de autenticación
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger para que soporte JWT
builder.Services.AddSwaggerGen(c =>
{
    // Definimos el esquema de seguridad JWT en Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce el token JWT así: Bearer {token}"
    });
    // Requerimos el token en todas las peticiones protegidas
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Primero autenticación y luego autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();