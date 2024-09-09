using Microsoft.AspNetCore.DataProtection;
using RedisCachingWebApi.Models;
using StackExchange.Redis;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);
var cacheTtl = builder.Configuration.GetValue<int>("CacheSettings:EmployeeTtlSeconds");

// Configure Serilog for logging
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day));

// Load Redis connection string from configuration
string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

// Register IConnectionMultiplexer as a singleton
try
{
    // Establish connection to Redis using the configured connection string
    var redis = ConnectionMultiplexer.Connect(redisConnectionString);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

    // Configure Data Protection to use Redis
    builder.Services.AddDataProtection()
        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
}
catch (RedisConnectionException ex)
{
    Log.Error("Could not connect to Redis: {Message}", ex.Message);
    throw;
}

// Register EmployeeRepository as a service
builder.Services.AddScoped<EmployeeRepository>();

// Register IDistributedCache to use Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "RedisInstance";
});

// Add controllers and necessary services
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Authorization services
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
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
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Commented out authentication since it's not currently in use
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
