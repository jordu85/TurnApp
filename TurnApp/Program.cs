using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TurnApp.Config;
using TurnApp.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); //esto lo tiene que completar LENA, tiene seguridad, cambiar nombre de api y descripcion



//servicios

//repositories

//mapper
builder.Services.AddAutoMapper(cfg => { }, typeof(Mapping));

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("devConnection"));
});

//jwt (LENA copia todo lo de seguridad)
//sting secret (el secreto use el mismo de la api empanadas)
//builder.Services.AddAuthentication
// .AddJwtBearer
// .AddCookie

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

var app = builder.Build();

//app.UseCors() LO COMPLETA LENA,ES SEGURIDAD

// Configure the HTTP request pipeline. esto debajo queda igual chicos, no toquen!
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
