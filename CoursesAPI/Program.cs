using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CoursesAPI.Data;
using Plain.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CoursesAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CoursesAPIContext") ?? throw new InvalidOperationException("Connection string 'CoursesAPIContext' not found.")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider(builder.Configuration["MessageBroker:Host"]));
builder.Services.AddScoped<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(), "enrollment_exchange", ExchangeType.Topic));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
