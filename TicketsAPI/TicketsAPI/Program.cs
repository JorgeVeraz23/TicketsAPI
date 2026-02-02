using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using System.Text;
using System.Text.Json.Serialization;
using TicketsAPI;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;
using TicketsAPI.Services;


var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;


// ============================
// Controllers + JSON Enum as string
// ============================
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();


// ============================
// DB Context
// ============================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ============================
// CORS
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================
// Repositories DI
// ============================
builder.Services.AddScoped<IEstudiante, EstudianteRepository>();
builder.Services.AddScoped<IAnioLectiivo, AnioLectivoRepository>();
builder.Services.AddScoped<IMateria, MateriaRepository>();
builder.Services.AddScoped<IMatricula, MatriculaRepository>();
builder.Services.AddScoped<IGrado, GradoRepository>();
builder.Services.AddScoped<IParalelo, ParaleloRepository>();
builder.Services.AddScoped<IGradoParalelo, OfertaRepository>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();  
builder.Services.AddScoped<IRepresentante, RepresentanteRepository>();
builder.Services.AddScoped<IRepresentanteService, RepresentanteService>();
builder.Services.AddScoped<IProfesor, ProfesorRepository>();
builder.Services.AddScoped<ICalificacion, CalificacionRepository>();
builder.Services.AddScoped<ITipoDocumento, TipoDocumentoRepository>();
builder.Services.AddScoped<IReportesServices, ReporteService>();
builder.Services.AddScoped<IMatriculaPdfService, MatriculaPdfService>();

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<DocumentoService>();

// ============================
// Identity
// ============================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ============================
// JWT Authentication + Authorization
// ============================
builder.Services.AddAuthorization();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Falta configurar Jwt:Key en appsettings.json");
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],

        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// ============================
// Swagger + Bearer
// ============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TicketsAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega SOLO el token JWT (sin escribir Bearer)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ============================
// Middleware pipeline
// ============================

// CORS
app.UseCors("AllowAll");


// Static files
app.UseStaticFiles();

// uploads folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});

// Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ============================
// Seed (roles + admin) + Apply migrations
// ============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // (Opcional) rol default
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    var admin = await userManager.FindByNameAsync("admin");
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@local.test",
            IsActive = true
        };

        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

app.MapControllers();

app.Run();
