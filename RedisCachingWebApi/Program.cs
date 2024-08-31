using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using RedisCachingWebApi.Models;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load Redis connection string from configuration
string redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

// Parse the Redis URI and configure the connection options
var redisUri = new Uri(redisConnectionString);
var redisConfig = new ConfigurationOptions
{
    EndPoints = { $"{redisUri.Host}:{redisUri.Port}" },
    AbortOnConnectFail = false,
    Ssl = false
};

// Check if UserInfo contains both username and password
if (!string.IsNullOrEmpty(redisUri.UserInfo))
{
    var userInfoParts = redisUri.UserInfo.Split(':');
    if (userInfoParts.Length == 2)
    {
        redisConfig.User = userInfoParts[0]; // Username
        redisConfig.Password = userInfoParts[1]; // Password
    }
}

// Register IConnectionMultiplexer as a singleton
try
{
    var redis = ConnectionMultiplexer.Connect(redisConfig);
    builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

    // Configure Data Protection to use Redis
    builder.Services.AddDataProtection()
        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
}
catch (RedisConnectionException ex)
{
    Console.WriteLine("Could not connect to Redis: " + ex.Message);
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

// Add Authentication services (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "YourIssuer",
            ValidAudience = "YourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey"))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
