using MotoGP_API.Configurations;
using MotoGP_API.Repositories;
using MotoGP_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Cloudinary desde appsettings.json
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);

// Registro de repositorios
builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<ICircuitoRepository, CircuitoRepository>();
builder.Services.AddScoped<IPilotoRepository, PilotoRepository>();

// Registro de servicios
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IMotoService, MotoService>();
builder.Services.AddScoped<ICircuitoService, CircuitoService>();
builder.Services.AddScoped<IPilotoService, PilotoService>();

// Registro del servicio de subida de imágenes
builder.Services.AddScoped<IUploadService, CloudinaryUploadService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();