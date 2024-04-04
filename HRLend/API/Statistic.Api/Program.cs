using Microsoft.OpenApi.Models;
using StatisticApi.Authorization;
using StatisticApi.Repository.DocumentDB;
using StatisticApi.Services.Queue.Consumer;
using StatisticApi.Settings;
using StatisticApi.Utils;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Statistic API", Version = "v1" });

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

string connectionStringMongoDB;
string mongoDB;
string queueUrl;


if (builder.Environment.IsDevelopment())
{
    connectionStringMongoDB = "mongodb://localhost:27017";
    queueUrl = "amqp://guest:guest@localhost:5672";
    mongoDB = "Statistic";
}
else
{
    //mongo
    var mongoDBHost = Environment.GetEnvironmentVariable("MONGO_DB_HOST");
    var mongoDBPort = Environment.GetEnvironmentVariable("MONGO_DB_PORT");
    mongoDB = Environment.GetEnvironmentVariable("MONGO_DB");
    var mongoDBUser = Environment.GetEnvironmentVariable("MONGO_DB_USER");
    var mongoDBPassword = Environment.GetEnvironmentVariable("MONGO_DB_PASSWORD");
    //queue
    var queueHost = Environment.GetEnvironmentVariable("QUEUE_HOST");
    var queuePort = Environment.GetEnvironmentVariable("QUEUE_PORT");
    var queueUser = Environment.GetEnvironmentVariable("QUEUE_USER");
    var queuePassword = Environment.GetEnvironmentVariable("QUEUE_PASSWORD");

    connectionStringMongoDB = $"mongodb://{mongoDBUser}:{mongoDBPassword}@{mongoDBHost}:{mongoDBPort}";
    queueUrl = $"amqp://{queueUser}:{queuePassword}@{queueHost}:{queuePort}";
}

builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>(ur => new StatisticRepository(connectionStringMongoDB, mongoDB));


builder.Services.AddHostedService(provider =>
    new StatisticConsumerService(
        queueUrl,
        new StatisticRepository(connectionStringMongoDB, mongoDB),
        provider.GetRequiredService<ILogger<StatisticConsumerService>>()
    )
);


var app = builder.Build();


app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
