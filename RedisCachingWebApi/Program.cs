using Microsoft.AspNetCore.DataProtection;
using RedisCachingWebApi.Application.Handlers;
using RedisCachingWebApi.Interface;
using RedisCachingWebApi.Repositories;
using RedisCachingWebApi.Swagger;
using Serilog;
using StackExchange.Redis;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Section: Logging with Serilog
// Configure Serilog to log to both the console and a file, with daily log rotation.
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day));

// Section: Redis Configuration
// Load the Redis connection string from the appsettings.json file.
string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

// Section: Redis Connection and Data Protection
// Establish a Redis connection and use Redis to store data protection keys.
try
{
    var redis = ConnectionMultiplexer.Connect(redisConnectionString);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

    // Configure ASP.NET Core Data Protection to persist keys to Redis.
    builder.Services.AddDataProtection()
        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
}
catch (RedisConnectionException ex)
{
    Log.Error("Could not connect to Redis: {Message}", ex.Message);
    throw;
}

// Section: SQL Server Connection
// Register IDbConnection with SQL Server using the connection string for the Employee database.
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("Employee")));

// Section: Dependency Injection for Repositories
// Register the repositories that handle database operations for employees and managers.
//builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();

// Section: Redis Caching
// Register Redis for distributed caching and set the Redis instance name.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "RedisInstance";
});

// Section: AutoMapper Configuration
// Register AutoMapper and scan the assemblies for mapping profiles.
builder.Services.AddAutoMapper(typeof(SaveManagerHandler).Assembly);  // Automatically scan for profiles

// Section: MediatR Configuration
// Register MediatR and scan the current assembly for command and query handlers.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SaveManagerHandler).Assembly));

// Section: Add MVC Controllers
// Register controllers with views (MVC pattern) in the services container.
builder.Services.AddControllersWithViews();

// Section: Swagger/OpenAPI
// Add services for API documentation generation using Swagger.
builder.Services.AddEndpointsApiExplorer();
// Section: Swagger/OpenAPI
// Add services for API documentation generation using Swagger.
builder.Services.AddSwaggerGen(options =>
{
    options.UseCustomSchemaIds();  // Use the custom schema ID strategy
    options.SchemaFilter<CustomSchemaIdStrategy>();  // Register the custom schema filter
});


// Section: Authorization
// Register a default authorization policy that requires authenticated users.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

// Section: Build and Configure the HTTP Request Pipeline
var app = builder.Build();

// Section: Development Environment
// Show detailed error pages and enable Swagger when in development mode.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    // Section: Production Environment
    // Use exception handler middleware and HTTP Strict Transport Security (HSTS) in production.
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Section: Middleware Configuration
// Enable HTTPS redirection and serving of static files.
app.UseHttpsRedirection();
app.UseStaticFiles();

// Section: Routing and Authorization
// Enable request routing and use authorization policies.
app.UseRouting();
app.UseAuthorization();

// Section: Map Controller Routes
// Map routes to controllers with default route configuration.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Section: Run the Application
// Start the web application.
app.Run();