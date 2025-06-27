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


var builder = WebApplication.CreateBuilder(args);

// ���������� �������� Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Event Platform API",
        Version = "v1",
        Description = "API ��� ��������� �����������",
        Contact = new OpenApiContact
        {
            Name = "���������",
            Email = "support@eventplatform.com"
        }
    });

    // ��������� JWT ����������� � Swagger
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

    // ���������� ���� � XML-������������ (���� �����)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ��������� middleware Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Platform API v1");

        // ��� ����������� ����� Swagger UI
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
    });
}

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // �������� ���� � XML-����� ������������
    var basePath = AppContext.BaseDirectory;
    var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
    var xmlPath = Path.Combine(basePath, $"{assemblyName}.xml");

    // ��������� ������������� ����� ����� �����������
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        // ����������� ��� �������
        var allFiles = Directory.GetFiles(basePath);
        Console.WriteLine($"Available files in {basePath}:");
        foreach (var file in allFiles)
        {
            Console.WriteLine(Path.GetFileName(file));
        }
    }
});

app.Run();