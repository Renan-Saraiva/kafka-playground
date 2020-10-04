using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Playground.Commun;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    public class ProducerWorker : BackgroundService
    {
        private readonly ILogger<ProducerWorker> _logger;

        public ProducerWorker(ILogger<ProducerWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = Constant.KafkaAdresses
                };

                using (var producer = new ProducerBuilder<long, string>(config).Build())
                {
                    _logger.LogInformation("Iniciando o envio de mensagens");

                    long count = 1;
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var message = new Message<long, string> {
                            Key = count,
                            Value = $"Mensagem de número {count}"
                        };

                        var result = await producer.ProduceAsync(
                            Constant.TopicName,
                            message
                        );

                        _logger.LogInformation($"Mensagem enviada: {message.Value} | " + $"Status: { result.Status }");
                        count++;

                        try
                        {
                            await Task.Delay(1000, stoppingToken);
                        }
                        catch (TaskCanceledException)
                        {
                            break;
                        }
                    }
                }

                _logger.LogInformation("Concluído o envio de mensagens");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exceção: {ex.GetType().FullName} | " + $"Mensagem: {ex.Message}");
            }
        }
    }
}
