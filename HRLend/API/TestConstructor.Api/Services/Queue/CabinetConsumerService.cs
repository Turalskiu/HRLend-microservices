
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using HRApi.Models.QueueMessage;
using System.Text.Json;
using System.Runtime.CompilerServices;
using HRApi.Repository.SqlDB;
using HRApi.Domain.Auth;
using System.Text.RegularExpressions;

namespace HRApi.Services.Queue
{
    public class CabinetConsumerService : BackgroundService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<CabinetConsumerService> _logger;
        private IConnection _connection;
        private IModel _channel;

        private string exchangeName = "auth_event";
        private string queueNameCabinet = "cabinet";
        private string routingKeyCabinet = "rout_cabinet";


        public CabinetConsumerService(
            string connectionString,
            IAuthRepository authRepository,
            ILogger<CabinetConsumerService> logger)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(connectionString);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(queue: queueNameCabinet, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueNameCabinet, exchange: exchangeName, routingKey: routingKeyCabinet);
            _channel.BasicQos(0, 1, false);

            _logger = logger;
            _authRepository = authRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerCabinet = new EventingBasicConsumer(_channel);
            consumerCabinet.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                CabinetQM cab = JsonSerializer.Deserialize<CabinetQM>(message);

                if (cab is not null)
                {
                    HandleMessage(cab);
                }

                _channel.BasicAck(ea.DeliveryTag, true);
            };

            _channel.BasicConsume(queue: queueNameCabinet, consumer: consumerCabinet);

            return Task.CompletedTask;

        }


        private void HandleMessage(CabinetQM message)
        {
            if(message.MessageType == (int)CABINET_MESSAGE_TYPE.ADD)
            {
                _authRepository.InsertCabinet(new Domain.Cabinet
                {
                    Id = message.CabinetId
                });
            }
            else if(message.MessageType == (int)CABINET_MESSAGE_TYPE.DELETE)
            {
                _authRepository.DeleteCabinet(message.CabinetId);
            }
        }
    }
}
