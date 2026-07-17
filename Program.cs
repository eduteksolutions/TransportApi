using TransportApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using TransportApi;
using static System.Net.WebRequestMethods;
using TransportApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TransportApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ================= SERVICES =================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();


builder.Services.AddHttpClient();
// ================= GPS SERVICES =================
builder.Services.AddScoped<GpsService>();
builder.Services.AddHostedService<GpsBackgroundService>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddSignalR();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddHttpClient<BusProximityService>();
builder.Services.AddScoped<BusProximityService>();

builder.Services.AddScoped<INotificationService, NotificationService>();
// ================= CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            // SetIsOriginAllowed(_ => true) allows all origins dynamically,
            // which safely permits you to chain .AllowCredentials()
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Required for SignalR streaming
        });
});

var app = builder.Build();


// ================= MIDDLEWARE =================
app.UseWebSockets();

app.UseCors("AllowAll");

app.UseAuthorization();

// ================= SWAGGER =================
app.UseSwagger();
app.UseSwaggerUI();

// ================= API =================
app.MapControllers();

// ================= SIGNALR HUB =================
app.MapHub<DataHub>("/dataHub");
app.MapHub<NotificationHub>("/notificationHub");

app.Run();

