using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using StudentsAPI;
using StudentsAPI.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StudentsAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudentsAPIContext") ?? throw new InvalidOperationException("Connection string 'StudentsAPIContext' not found.")));

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider(builder.Configuration["MessageBroker:Host"]));
builder.Services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(), "enrollment_exchange", "enrollment_queue", "enrollment_topic", ExchangeType.Topic));
builder.Services.AddHostedService<EnrollmentDataCollector>();

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
