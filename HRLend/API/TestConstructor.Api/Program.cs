using HRApi.Authorization;
using HRApi.Repository.gRPC;
using HRApi.Repository.SqlDB;
using HRApi.Services;
using HRApi.Services.Queue;
using HRApi.Settings;
using HRApi.Utils;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddCors();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HR API", Version = "v1" });

    // Включение поддержки аннотаций из Swashbuckle.AspNetCore.Annotations
    c.EnableAnnotations();

    // Добавьте описание схемы безопасности (например, JWT Bearer Token)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Добавьте возможность использовать схему безопасности для каждого метода в Swagger UI
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


builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));
var connectionString = "Host=localhost;Port=5432;Database=HR;Username=postgres;Password=qweasdzxc123987";
var testModuleGrpcUrl = "http://localhost:5271";
var knowledgeBaseGrpcUrl = "http://localhost:5000";

var queueUrl = "amqp://guest:guest@localhost:5672";

// configure DI for application services
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<ITestTemplateRepository, TestTemplateRepository>(ur => new TestTemplateRepository(connectionString));
builder.Services.AddScoped<ICompetenceRepository, CompetenceRepository>(ur => new CompetenceRepository(connectionString));
builder.Services.AddScoped<ISkillRepository, SkillRepository>(ur => new SkillRepository(connectionString));
builder.Services.AddScoped<IAuthRepository, AuthRepository>(ur => new AuthRepository(connectionString));
builder.Services.AddScoped<ITestModuleRepository, TestModuleRepository>(ur => new TestModuleRepository(testModuleGrpcUrl));
builder.Services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>(ur => new KnowledgeBaseRepository(knowledgeBaseGrpcUrl));


builder.Services.AddHostedService(provider =>
    new CabinetConsumerService(
        queueUrl,
        new AuthRepository(connectionString),
        provider.GetRequiredService<ILogger<CabinetConsumerService>>()
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.MapGrpcService<TemplateService>();

app.Run();

