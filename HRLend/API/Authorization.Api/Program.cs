using AuthorizationApi.Authorization;
using AuthorizationApi.Services;
using AuthorizationApi.Settings;
using AuthorizationApi.Utils;
using AuthorizationApi.Repository;
using System.Reflection;
using Microsoft.OpenApi.Models;
using AuthorizationApi.Services.Queue;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors();

builder.Services.AddControllers();
    //.AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
    

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });

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


// configure strongly typed settings object
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSettings"));
var connectionString = "Host=localhost;Port=5432;Database=Auth;Username=postgres;Password=qweasdzxc123987";
var url = "Resources";

var queueUrl = "amqp://guest:guest@localhost:5672";


// configure DI for application services
builder.Services.AddScoped<IUserRepository, UserRepository>(ur => new UserRepository(connectionString));
builder.Services.AddScoped<IAdminRepository, AdminRepository>(ur => new AdminRepository(connectionString));
builder.Services.AddScoped<ICabinetRepository, CabinetRepository>(ur => new CabinetRepository(connectionString));
builder.Services.AddScoped<IObjectStoreRepository, FolderRepository>(ur => new FolderRepository(url));
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthPublisherService, AuthPublisherService>(ur => new AuthPublisherService(queueUrl));


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

// global error handler
//app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
