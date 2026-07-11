using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resend;
using System;
using System.Text;
using TurnApp.Config;
using TurnApp.Models.Especialidad;
using TurnApp.Models.Paciente;
using TurnApp.Models.Profesional;
using TurnApp.Models.Role;
using TurnApp.Models.Turno;
using TurnApp.Repositories;
using TurnApp.Services;
using TurnApp.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TurnApp - System API",
        Description = "Gestiona tus turnos online",
        TermsOfService = new Uri("https://www.turnapp.com"),
    });
    options.AddSecurityDefinition("token", new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Description = "Json Web Token, Cabecera de Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Name = "Authorization",
        Scheme = "bearer",
    });
    options.OperationFilter<AuthOperationFilter>();
}); 


//servicios

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EspecialidadService>();
builder.Services.AddScoped<TurnoService>();
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<ProfesionalService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEncoderService, EncoderService>();

//repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();

builder.Services.AddScoped<IRepository<Especialidad>, Repository<Especialidad>>();
builder.Services.AddScoped<IRepository<Turno>, Repository<Turno>>();
builder.Services.AddScoped<IRepository<Paciente>, Repository<Paciente>>();
builder.Services.AddScoped<IRepository<Profesional>, Repository<Profesional>>();

//mapper
builder.Services.AddAutoMapper(cfg => { }, typeof(Mapping));

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("devConnection"));
});

//jwt 
string secret = builder.Configuration
    .GetSection("Secrets:jwt")?.Value?.ToString()
    ?? throw new Exception("invalid jwt secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(secret);
        options.SaveToken = true;
        options.TokenValidationParameters =
            new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
    })
    .AddCookie(opt =>
    {
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opt.Cookie.SameSite = SameSiteMode.None;
        opt.Cookie.IsEssential = true; // esto es para que, aunque el usuario no acepte las cookies, se setee igual.
        opt.ExpireTimeSpan = TimeSpan.FromDays(1);
    });


// Filter
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
        .Where(x => x.Value?.Errors.Count > 0)
        .ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
        );
        ResponseValidation validation = new ResponseValidation(errors);
        return new BadRequestObjectResult(validation);
    };
});


// Email
builder.Services.AddResend(o =>
{
    o.ApiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN")!;
});

var app = builder.Build();

//cors
app.UseCors(options =>
{
    options.WithOrigins("http://localhost:5173");
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowCredentials();
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
