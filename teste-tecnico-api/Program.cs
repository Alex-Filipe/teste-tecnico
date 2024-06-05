using Microsoft.EntityFrameworkCore;
using teste_tecnico_api.src.Database;
using teste_tecnico_api.src.Interfaces;
using teste_tecnico_api.src.Repositories;
using teste_tecnico_api.src.Services;
using teste_tecnico_api.src.Services.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<BillToPayService>();
builder.Services.AddScoped<IBillToPayRepository, BillToPayRepository>();
builder.Services.AddScoped<IBillToPayValidator, BillToPayValidator>();

// CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000","http://localhost:4200")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod();
                      });
});

//Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
