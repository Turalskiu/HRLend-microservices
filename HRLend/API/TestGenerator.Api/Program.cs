using TestGeneratorApi.Repository;
using TestGeneratorApi.Services;
using TestGeneratorApi.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();

builder.Services.AddGrpc();

string connectionString = "mongodb://localhost:27017";
string db = "TG";

if (builder.Environment.IsDevelopment())
{
    connectionString = "mongodb://localhost:27017";
    db = "TG";
}
else
{
    var dbHost = Environment.GetEnvironmentVariable("MONGO_DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("MONGO_DB_PORT");
    db = Environment.GetEnvironmentVariable("MONGO_DB");
    var dbUser = Environment.GetEnvironmentVariable("MONGO_DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("MONGO_DB_PASSWORD");

    connectionString = $"mongodb://{dbUser}:{dbPassword}@{dbHost}:{dbPort}";
}

builder.Services.AddScoped<ITestModuleRepository, TestModuleRepository>(ur => new TestModuleRepository(connectionString, db));


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());


app.MapGrpcService<TestModuleService>();
app.MapGrpcService<TestGeneratorService>();

//инициализируем все маппинги
MapUtil.Init();

app.Run();
