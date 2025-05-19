
using Application.Handlers;
using Application.Interfaces;
using Infrastructure.data;
using Infrastructure.Kafka;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AppDbContext>(options =>

options.UseNpgsql(
    builder.Configuration.GetConnectionString("Postgres")
));

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));


builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IKafka, KafkaP>();
builder.Services.AddHostedService<KafkaC>();
builder.Services.AddScoped<CreateTransactionHandler>();
builder.Services.AddScoped<GetTransactionByIdHandler>();
builder.Services.AddScoped<GetDailyTotalHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
