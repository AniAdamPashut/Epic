using System.Reactive.Linq;
using Epic.Abstract;
using Epic.Serialization;
using Epic.Rabbitmq;
using Epic.Extensions;

var builder = WebApplication.CreateBuilder(args);

const string SOURCE = "rabbitmq_source";

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection(nameof(RabbitMqConfig)));
builder.Services.AddSingleton<IDeserializer<string>, UTF8Serializer>();
builder.Services.AddKeyedSingleton<RabbitMqConsumerService<string>>(SOURCE);
builder.Services.AddHostedService(sp => sp.GetKeyedService<RabbitMqConsumerService<string>>(SOURCE)!);

var app = builder.Build();

var source = app.Services.GetKeyedService<RabbitMqConsumerService<string>>(SOURCE)!;

source
    .Filter(x => x.StartsWith("aa"), "Starts with aa")
    .FlatMap(x =>
    {
        var values = x.Split(' ');
        var guid = Guid.NewGuid();
        return values.Select(val => (val, guid)).ToList();
    })
    .KeyBy(x => x.guid)
    .Acknowledge((x, key) => Console.WriteLine($"{x.val}, {key}"));

app.Run();
