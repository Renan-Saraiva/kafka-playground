using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Playground.Commun;

namespace Consumer
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly ILogger<ConsumerWorker> _logger;

        public ConsumerWorker(ILogger<ConsumerWorker> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Constant.KafkaAdresses,
                GroupId = $"{Constant.TopicName}-group-0",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _logger.LogInformation("Iniciando escuta das mensagens");

            try
            {
                using (var consumer = new ConsumerBuilder<long, string>(config).Build())
                {
                    consumer.Subscribe(Constant.TopicName);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try 
                        {
                            var cr = consumer.Consume(stoppingToken);                            
                            _logger.LogInformation($"Mensagem recebida: {cr.Message.Value} | Chave: {cr.Message.Key}");
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogWarning("Cancelada a execução do Consumer...");
                            break;
                        }
                    }

                    consumer.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exceção: {ex.GetType().FullName} | " + $"Mensagem: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
