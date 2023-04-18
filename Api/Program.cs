using BL.Common;
using BL.Models;
using BL.Repositories.Implements;
using BL.Repositories;
using BL.Services.Implements;
using BL.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Api.Helpers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Variables de configuración
var configuration = builder.Configuration;
var appSettingsSection = configuration.GetSection("AppSettings");
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

//Configuración para la inyección del perfil de automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Configuración del cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ConfigCors", builder =>
    {
        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
        .AllowAnyHeader().AllowAnyMethod();
    });
});

//Configuración para agregar el dbcontext, repositorios y servicios
builder.Services.AddDbContext<DbSuscripcionEventosContext>(options => options.UseSqlServer(configuration.GetConnectionString("connectionString")));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ISuscripcionRepository, SuscripcionRepository>();
builder.Services.AddScoped<ISuscripcionService, SuscripcionService>();
builder.Services.Configure<AppSettings>(appSettingsSection);

//Configuración para el filtro de entidades no procesadas y deshabilitar el comportamiento 400 automático
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);

//Configuracion de la autenticación basada en JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(z =>
{
    z.RequireHttpsMetadata = false;
    z.SaveToken = true;
    z.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    //Configuración para las propiedades que inicien con letra mayúscula
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=> c.MapType<TimeSpan>(() => new OpenApiSchema
{
    Type = "string",
    Example = new OpenApiString("00:00:00")
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseCors("ConfigCors");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
