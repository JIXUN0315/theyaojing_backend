using Backend.DILibrary;
using Backend.Environment;
using System.Threading.RateLimiting;
using System.Text;
using Backend.EnvironmentOptions;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
DotNetEnv.Env.Load(); 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var supabaseConn = Environment.GetEnvironmentVariable("SUPABASE_CONN");

builder.Services.AddScoped<IDbConnection>(sp => {
    if(string.IsNullOrEmpty(supabaseConn))
        throw new Exception("SUPABASE_CONN not found in environment variables.");

    return new NpgsqlConnection(supabaseConn);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "http://localhost:5173",  
                "https://theyaojing.org" 
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});


// 限制 同一個 ip 在短時間內不能練續送請求
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("FormSubmitPolicy", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
    
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Try again later.", token);
    };
});

builder.Services.AddRDS(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.Configure<FormNotifyOptions>(
    builder.Configuration.GetSection("FormNotify"));
builder.Services.Configure<JwtOptions>(
        builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CloudinaryOptions>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])
            )
        };
    });


var app = builder.Build();
app.UseDefaultFiles();      
app.UseStaticFiles();   
app.UseRouting();

app.UseCors("AllowFrontend");  
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 &&
        !System.IO.Path.HasExtension(context.Request.Path.Value) &&
        !context.Request.Path.Value.StartsWith("/api"))
    {
        context.Request.Path = "/index.html";
        context.Response.StatusCode = 200;
        await next();
    }
});

app.MapControllers();
app.Run();
