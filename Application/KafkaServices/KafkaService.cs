using Confluent.Kafka;
using Domain.Interface.KafkaServices;
using Infrastructure.Audilog;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.KafkaServices
{
    public class KafkaService : IKafkaProducer, IKafkaConsumer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _brokerList;
        private readonly ILogger<KafkaService> _logger;
        public KafkaService(string brokerList, ILogger<KafkaService> logger)
        {
            var config = new ProducerConfig { BootstrapServers = brokerList };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _brokerList = brokerList;
            _logger = logger;
        }

        public async Task ProduceAsync<T>(string topic, T value, string key = null)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = key,
                    Value = JsonSerializer.Serialize(value),
                };

                await _producer.ProduceAsync(topic, message);
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError($"Delivery failed: {e.Error.Reason}");
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }

        public async Task ConsumeAsync<T>(string topic, Action<T> onMessageReceived, string groupId = null)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _brokerList,
                GroupId = groupId ?? Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume();

                        var value = JsonSerializer.Deserialize<T>(consumeResult.Message.Value);

                        onMessageReceived(value);

                        consumer.Commit(consumeResult);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"Consumer failed: {ex.Message}");
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
                }
            }
        }

        public async Task<IEnumerable<T>> ConsumeAsync<T>(string topic, string groupId = null)
        {
            var messages = new List<T>();

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _brokerList,
                GroupId = groupId ?? Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));
                        if (consumeResult == null)
                        {
                            break; // No message was received within the timeout period.
                        }
                        var value = JsonSerializer.Deserialize<T>(consumeResult.Message.Value);
                        messages.Add(value);
                        consumer.Commit(consumeResult);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"Consumer failed: {ex.Message}");
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
                }
            }

            return messages;
        }
    }
}
