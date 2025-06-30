using EventPlatform.Data;
using EventPlatform.Services;
using EventPlatform.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using EventPlatform.Web.Services;
using EventPlatform.Web.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using EventPlatform.Controllers;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(TicketsController).Assembly));
// Добавление сервисов Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Event Platform API",
        Version = "v1",
        Description = "API для платформы мероприятий",
        Contact = new OpenApiContact
        {
            Name = "Поддержка",
            Email = "support@eventplatform.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// Настройка middleware Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Platform API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseRouting();
app.Run();