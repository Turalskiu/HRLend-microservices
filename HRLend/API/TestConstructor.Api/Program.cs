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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Constructor API", Version = "v1" });

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

string connectionStringDB;
string testModuleGrpcUrl;
string knowledgeBaseGrpcUrl;
string queueUrl;

if (builder.Environment.IsDevelopment())
{
    connectionStringDB = "Host=localhost;Port=5432;Database=HR;Username=postgres;Password=qweasdzxc123987";
    testModuleGrpcUrl = "http://localhost:5204";
    knowledgeBaseGrpcUrl = "http://localhost:8081";
    queueUrl = "amqp://guest:guest@localhost:5672";
}
else
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
    var db = Environment.GetEnvironmentVariable("DB");
    var dbUser = Environment.GetEnvironmentVariable("DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    var tgUrl = Environment.GetEnvironmentVariable("TG_URL");
    var tgPort = Environment.GetEnvironmentVariable("TG_PORT");
    var kbUrl = Environment.GetEnvironmentVariable("KB_URL");
    var kbPort = Environment.GetEnvironmentVariable("KB_PORT");
    var queueHost = Environment.GetEnvironmentVariable("QUEUE_HOST");
    var queuePort = Environment.GetEnvironmentVariable("QUEUE_PORT");
    var queueUser = Environment.GetEnvironmentVariable("QUEUE_USER");
    var queuePassword = Environment.GetEnvironmentVariable("QUEUE_PASSWORD");

    testModuleGrpcUrl = $"http://{tgUrl}:{tgPort}";
    knowledgeBaseGrpcUrl = $"http://{kbUrl}:{kbPort}";
    connectionStringDB = $"Host={dbHost};Port={dbPort};Database={db};Username={dbUser};Password={dbPassword}";
    queueUrl = $"amqp://{queueUser}:{queuePassword}@{queueHost}:{queuePort}";
}


// configure DI for application services
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<ITestTemplateRepository, TestTemplateRepository>(ur => new TestTemplateRepository(connectionStringDB));
builder.Services.AddScoped<ICompetenceRepository, CompetenceRepository>(ur => new CompetenceRepository(connectionStringDB));
builder.Services.AddScoped<ISkillRepository, SkillRepository>(ur => new SkillRepository(connectionStringDB));
builder.Services.AddScoped<IAuthRepository, AuthRepository>(ur => new AuthRepository(connectionStringDB));
builder.Services.AddScoped<ITestModuleRepository, TestModuleRepository>(ur => new TestModuleRepository(testModuleGrpcUrl));
builder.Services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>(ur => new KnowledgeBaseRepository(knowledgeBaseGrpcUrl));

builder.Services.AddHostedService(provider =>
    new CabinetConsumerService(
        queueUrl,
        new AuthRepository(connectionStringDB),
        provider.GetRequiredService<ILogger<CabinetConsumerService>>()
    )
);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.MapGrpcService<TemplateService>();

app.Run();

