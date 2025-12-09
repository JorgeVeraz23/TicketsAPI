using Microsoft.EntityFrameworkCore;
using TicketsAPI;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;
using System.Text.Json.Serialization;
using Microsoft.Extensions.FileProviders;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))

    );

// Agregar política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(["http://localhost:5173", "http://localhost:3000"]) // URL de tu aplicación React
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddScoped<UsuarioInterface, UsuarioRepository>();
builder.Services.AddScoped<FormInterface, FormRepository>();
builder.Services.AddScoped<SolicitudInterface, SolicitudRepository>();
builder.Services.AddScoped<FormGroupInterface, FormGroupRepository>();
builder.Services.AddScoped<FormFieldInterface, FormFieldRepository>();
builder.Services.AddScoped<OptionInterface, OptionRepository>();
builder.Services.AddScoped<FieldTypeInterface, FieldTypeRepository>();
builder.Services.AddScoped<ICurso, CursoRepository>();
builder.Services.AddScoped<IRepresentate, RepresentateRepository>();
builder.Services.AddScoped<IEstudiante, EstudianteRepository>();
builder.Services.AddScoped<IProfesor, ProfesorRepository>();


// Agregar servicios al contenedor.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


// Agregar el contexto de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Matriculas AMA",  // Cambia esto por el nombre que desees
        Version = "v1",
        Description = "Sistema de gestion de matriculas para plantel educativo"
    });
});



var app = builder.Build();

// Usar la política de CORS configurada
app.UseCors("AllowReactApp");

app.UseStaticFiles(); // Permite servir archivos estáticos

// Agrega esta línea para especificar la carpeta "uploads"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
