using AntiFraudService;
using AntiFraudService.config;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
