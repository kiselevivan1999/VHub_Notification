using Application.Contracts.Commands;
using Application.Contracts.Requests;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Implementation.Consumers;

public class NotifiсationConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotifiсationConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public NotifiсationConsumer(IConfiguration configuration,
        ILogger<NotifiсationConsumer> logger, IServiceProvider serviceProvider) 
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Костыль. Без него не инициализируются контроллеры и api недоступно.
        await Task.Delay(10000, stoppingToken);

        _logger.LogInformation("Старт обработки");

        var kafkaSection = _configuration.GetSection("Kafka");
        if (kafkaSection == null)
            throw new Exception("Конфигурация для Kafka не найдена.");

        var brokers = kafkaSection.GetSection("Brokers").Get<string[]>();
        var groupId = kafkaSection["GroupId"];
        var topic = kafkaSection["Topic"];

        var config = new ConsumerConfig
        {
            BootstrapServers = string.Join(',', brokers),
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
            EnablePartitionEof = true,
            AllowAutoCreateTopics = true,
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config)
            .SetErrorHandler((_, error) =>
                _logger.LogError("Kafka error: {Reason}", error.Reason))
            .SetPartitionsAssignedHandler((_, partitions) =>
            {
                _logger.LogInformation("Assigned partitions: {Partitions}",
                    string.Join(", ", partitions));
            })
            .Build();

        consumer.Subscribe(topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                if (result is { IsPartitionEOF: false })
                {
                    var json = result.Message.Value;
                    var request = JsonSerializer.Deserialize<SendNotificationRequest>(json);

                    if (request == null)
                    {
                        _logger.LogError("Ошибка десерилизации сообщения {EventName}: {Json}", 
                            nameof(SendNotificationRequest), json);
                        return;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send(new SendNotificationCommand(request), stoppingToken);
                    consumer.Commit(result);
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogError(e, "Consume error");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer stopped");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing error");
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
    }
}
