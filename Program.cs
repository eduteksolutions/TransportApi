using TransportApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using TransportApi;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ================= SERVICES =================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

// ================= CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
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

app.Run();