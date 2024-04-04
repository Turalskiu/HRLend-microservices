using KnowledgeBaseApi.Authorization;
using KnowledgeBaseApi.Repository.DocumentDB;
using KnowledgeBaseApi.Services;
using KnowledgeBaseApi.Settings;
using KnowledgeBaseApi.Utils;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddCors();

builder.Services.AddControllers();
//.AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KB API", Version = "v1" });

    // ��������� ��������� ��������� �� Swashbuckle.AspNetCore.Annotations
    c.EnableAnnotations();

    // �������� �������� ����� ������������ (��������, JWT Bearer Token)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // �������� ����������� ������������ ����� ������������ ��� ������� ������ � Swagger UI
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
            new string[] {}
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


// configure strongly typed settings object
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));

string connectionStringMongoDB;
string tgUrl;
string mongodb;

if (builder.Environment.IsDevelopment())
{
    connectionStringMongoDB = "mongodb://localhost:27017";
    mongodb = "KB";
}
else
{
    var dbHost = Environment.GetEnvironmentVariable("MONGO_DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("MONGO_DB_PORT");
    mongodb = Environment.GetEnvironmentVariable("MONGO_DB");
    var dbUser = Environment.GetEnvironmentVariable("MONGO_DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("MONGO_DB_PASSWORD");
    var tgHost = Environment.GetEnvironmentVariable("TG_URL");
    var tgPort = Environment.GetEnvironmentVariable("TG_PORT");

    tgUrl = $"http://{tgHost}:{tgPort}";
    connectionStringMongoDB = $"mongodb://{dbUser}:{dbPassword}@{dbHost}:{dbPort}";
}


// configure DI for application services
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IProfessionRepository, ProfessionRepository>(ur => new ProfessionRepository(connectionStringMongoDB, mongodb));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();



app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// global error handler
//app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.MapGrpcService<CopyingDataService>();


app.Run();
