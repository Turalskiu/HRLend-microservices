using Contracts.Authorization.Queue;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AuthorizationApi.Services.Queue
{
    public interface IAuthPublisherService
    {
        void UserMessage(UserQM message);
        void GroupMessage(GroupQM message);
        void CabinetMessage(CabinetQM message);
    }

    public class AuthPublisherService : IAuthPublisherService
    {
        private readonly string _connectionString;

        public AuthPublisherService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void UserMessage(UserQM message)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_connectionString);

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchangeName = "auth_event";
            string queueName = "user";
            string routingKey = "rout_user";

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }


        public void GroupMessage(GroupQM message)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_connectionString);

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchangeName = "auth_event";
            string queueName = "group";
            string routingKey = "rout_group";

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }


        public void CabinetMessage(CabinetQM message)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_connectionString);

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchangeName = "auth_event";
            string queueName = "cabinet";
            string routingKey = "rout_cabinet";

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
