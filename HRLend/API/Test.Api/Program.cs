using TestApi.Authorization;
using Microsoft.OpenApi.Models;
using System.Reflection;
using HRApi.Utils;
using TestApi.Settings;
using TestApi.Repository.DocumentDB;
using TestApi.Repository.GRPC;
using TestApi.Repository.SqlDB;
using TestApi.Services;
using Microsoft.Extensions.DependencyInjection;
using TestApi.Services.Queue.Consumer;
using TestApi.Services.Queue.Publisher;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(2*3600);
    options.Cookie.IsEssential = true;
});
builder.Services.AddGrpc();
builder.Services.AddCors();
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });

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
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSettings"));

var connectionString = "Host=localhost;Port=5432;Database=Test;Username=postgres;Password=qweasdzxc123987";
var connectionStringMongoDB = "mongodb://localhost:27017";
var templateGrpcUrl = "http://localhost:4999";
var testGenerateGrpcUrl = "http://localhost:5271";

var queueUrl = "amqp://guest:guest@localhost:5672";

// configure DI for application services
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<TestApi.Repository.SqlDB.ITestRepository, TestApi.Repository.SqlDB.TestRepository>(ur => new TestApi.Repository.SqlDB.TestRepository(connectionString));
builder.Services.AddScoped<ILinkRepository, LinkRepository>(ur => new LinkRepository(connectionString));
builder.Services.AddScoped<IAuthRepository, AuthRepository>(ur => new AuthRepository(connectionString));
builder.Services.AddScoped<TestApi.Repository.DocumentDB.ITestRepository, TestApi.Repository.DocumentDB.TestRepository>(ur => new TestApi.Repository.DocumentDB.TestRepository(connectionStringMongoDB, "Test"));
builder.Services.AddScoped<ITestTemplateRepository, TestTemplateRepository>(ur => new TestTemplateRepository(templateGrpcUrl));
builder.Services.AddScoped<ITestGeneratorRepository, TestGeneratorRepository>(ur => new TestGeneratorRepository(testGenerateGrpcUrl));
builder.Services.AddScoped<ITemplateStatisticsService, TemplateStatisticsService>(ur => new TemplateStatisticsService());
builder.Services.AddScoped<IStatisticPublisherService, StatisticPublisherService>(ur => new StatisticPublisherService(queueUrl));


builder.Services.AddHostedService(provider =>
    new AuthConsumerService(
        queueUrl,
        new AuthRepository(connectionString),
        provider.GetRequiredService<ILogger<AuthConsumerService>>()
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());


app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();

