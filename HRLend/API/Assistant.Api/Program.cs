using Microsoft.OpenApi.Models;
using AssistantApi.Middleware;
using AssistantApi.Settings;
using AssistantApi.Utils;
using System.Reflection;
using Assistant.Api.Repository;
using Assistant.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Assistant API", Version = "v1" });

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

builder.Services.AddCors();
builder.Services.AddControllers();

// configure strongly typed settings object
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));

string connectionStringSqlDB;
string chadGptUrl;
string chadGptApiKey;
string elasticsearchUrl;
string elasticsearchUsername;
string elasticsearchPassword;
string elasticsearchApiKeyId;
string elasticsearchApiKey;
string elasticsearchIndex;


connectionStringSqlDB = "Host=localhost;Port=5432;Database=Assistant;Username=postgres;Password=qweasdzxc123987";
chadGptUrl = "https://ask.chadgpt.ru/api/public/gpt-3.5";
chadGptApiKey = "chad-c647e5d4d8fa4f0ca5457d4e62f965812ra1iudl";
elasticsearchUrl = "https://localhost:9200";
elasticsearchUsername = "elastic";
elasticsearchPassword ="qweasdzxc123987";
elasticsearchIndex = "document_index";

//await ElasticsearchRepository.DeleteIndex(elasticsearchUrl, elasticsearchUsername, elasticsearchPassword, elasticsearchIndex);
await ElasticsearchRepository.CreateIndex(elasticsearchUrl, elasticsearchUsername, elasticsearchPassword, elasticsearchIndex);


builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>(ur => new DocumentRepository(connectionStringSqlDB));
builder.Services.AddScoped<IElasticsearchRepository, ElasticsearchRepository>(ur => new ElasticsearchRepository(elasticsearchUrl, elasticsearchUsername, elasticsearchPassword, elasticsearchIndex));
builder.Services.AddScoped<IGptService, ChadGptService>(ur => new ChadGptService(chadGptUrl, chadGptApiKey));
builder.Services.AddScoped<ISplitDocumentService, SimpleSplitDocumentService>();


var app = builder.Build();


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


app.Run();

