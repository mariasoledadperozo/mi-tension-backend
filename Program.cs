using mi_tension_backend.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la Base de Datos (PostgreSQL - Supabase)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MiDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// 2. Registro de servicios básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Configuración del Pipeline de HTTP (Swagger y Rutas)
// Esto es lo que hace que Swagger funcione al abrir el navegador
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi Tension API V1");
        // Deja el RoutePrefix vacío para que Swagger cargue en la raíz (http://localhost:XXXX/)
        c.RoutePrefix = string.Empty;
    });
}

// Middlewares necesarios para seguridad y mapeo de rutas
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();