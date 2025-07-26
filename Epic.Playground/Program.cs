using System.Reactive.Linq;
using Epic.Serialization;
using Epic.Rabbitmq;
using Epic.Extensions;
using Epic.Abstract.Serialization;
using Epic.Playground.Filters;
using Epic.Playground.Processors;

var builder = WebApplication.CreateBuilder(args);

const string SOURCE = "rabbitmq_source";

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection(nameof(RabbitMqConfig)));
builder.Services.AddSingleton<IDeserializer<string>, UTF8Serializer>();
builder.Services.AddKeyedSingleton<RabbitMqConsumerService<string>>(SOURCE);
builder.Services.AddHostedService(sp => sp.GetKeyedService<RabbitMqConsumerService<string>>(SOURCE)!);

var app = builder.Build();

var source = app.Services.GetKeyedService<RabbitMqConsumerService<string>>(SOURCE)!;

var aaFilterFunction = new AAFilterFunction();
var splitBySpaceFlatMapFunction = new SplitBySpaceFlatMapFunction();
var delayByLengthAsyncMapFunction = new DelayByLengthAsyncMapFunction();
var multiplyStringMapFunction = new MultiplyStringMapFunction();

source
    .Filter(aaFilterFunction)
    .FlatMap(splitBySpaceFlatMapFunction)
    .MapAsync(delayByLengthAsyncMapFunction)
    .Map(multiplyStringMapFunction)
    .KeyBy(pair => pair.Guid)
    .Acknowledge((x, key) => Console.WriteLine($"{x.Value}, {key}"));

app.Run();
