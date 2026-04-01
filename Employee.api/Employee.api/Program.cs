using Employee.api.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("enableAll", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});
builder.Services.AddDbContext<EmployeeDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("empCon")));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("enableAll");

app.MapControllers();

app.Run();
