using Microsoft.EntityFrameworkCore;
using Forwarty.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var cs = builder.Configuration.GetConnectionString("MySql")!;
builder.Services.AddDbContext<ForwartyDbContext>(opt =>
{
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs));

    // Ponlo en true en appsettings.json para ver el SQL que genera EF.
    if (builder.Configuration.GetValue<bool>("MostrarSql"))
        opt.LogTo(Console.WriteLine, LogLevel.Information);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

Console.WriteLine("API lista en http://localhost:5080  (Swagger en /swagger)");

app.Run();
