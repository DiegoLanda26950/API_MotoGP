using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MotoGP_API.Configurations;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Cloudinary desde appsettings.json
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);

// Configuración de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            // Validamos el emisor del token
            ValidateIssuer = true,
            // Validamos la audiencia del token
            ValidateAudience = true,
            // Validamos la fecha de expiración
            ValidateLifetime = true,
            // Validamos la firma del token
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
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
builder.Services.AddScoped<IAuthService, AuthService>();

// Registro del servicio de subida de imágenes
builder.Services.AddScoped<IUploadService, CloudinaryUploadService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger para que soporte JWT
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MotoGP_API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT así: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
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

// Primero autenticación y luego autorización (el orden importa)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();