using System.Reactive.Linq;
using Epic;
using Epic.Abstract;
using Epic.Serialization;
using Epic.Rabbitmq;

var builder = WebApplication.CreateBuilder(args);

const string SOURCE = "rabbitmq_source";

builder.Services.Configure<RabbitmqConfig>(builder.Configuration.GetSection(nameof(RabbitmqConfig)));
builder.Services.AddSingleton<IDeserializer<string>, UTF8Serializer>();
builder.Services.AddKeyedSingleton<RabbitmqConsumerService<string>>(SOURCE);
builder.Services.AddHostedService(sp => sp.GetKeyedService<RabbitmqConsumerService<string>>(SOURCE)!);

var app = builder.Build();

var source = app.Services.GetKeyedService<RabbitmqConsumerService<string>>(SOURCE)!;

source
    .Filter(x => x.StartsWith("aa"), "Starts with aa")
    .Map(x => $"{x}{x}")
    .Acknowledge(x => Console.WriteLine(x));

app.Run();
