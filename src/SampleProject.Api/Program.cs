using FluentValidation;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using SampleProject.Application.Mediator;
using SampleProject.Application.Validators;
using SampleProject.Domain.Interfaces;
using SampleProject.Infrastructure.Cache;
using SampleProject.Infrastructure.Encryption;
using SampleProject.Infrastructure.Persistence.DbConnection;
using SampleProject.Infrastructure.Persistence.Repositories;
using SampleProject.Api.Middleware;
using SampleProject.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog 設定（必須在 CreateBuilder 之後立即設定）
builder.AddSerilog();

// 服務註冊
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SampleProject API",
        Version = "v1",
        Description = "A .NET 8 Web API with DDD, Clean Architecture, and Custom Mediator"
    });
});

// Encryption Service
builder.Services.AddSingleton<IEncryptionService, Aes256EncryptionService>();

// Database Connection Factory
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

// Redis Cache
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis connection string is not configured.");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISkuRepository, SkuRepository>();
builder.Services.AddScoped<IStorageRepository, StorageRepository>();
builder.Services.AddScoped<ISpecificationRepository, SpecificationRepository>();
builder.Services.AddScoped<ISpecificationValueRepository, SpecificationValueRepository>();
builder.Services.AddScoped<ISkuSpecificationRepository, SkuSpecificationRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

// Mediator
builder.Services.AddScoped<IMediator, Mediator>();

// 自動註冊所有 Handlers
builder.Services.RegisterRequestHandlers();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 中介軟體管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
finally
{
    // 確保 Serilog 正確關閉
    Log.CloseAndFlush();
}
