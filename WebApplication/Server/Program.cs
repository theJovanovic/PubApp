using Microsoft.EntityFrameworkCore;
using Models;

var AllowAll = "AllowAll";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy
    (
        name: AllowAll,
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

builder.Services.AddDbContext<PubContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PubDB"))
);

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowAll);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
