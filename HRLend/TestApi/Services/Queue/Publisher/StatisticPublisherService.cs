using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TestApi.Domain.QueueMessage.Auth;
using TestApi.Domain.QueueMessage.Statistic;

namespace TestApi.Services.Queue.Publisher
{
    public interface IStatisticPublisherService
    {
        void StatisticMessage(UserStatisticQM message);
    }


    public class StatisticPublisherService : IStatisticPublisherService
    {
        private readonly string _connectionString;

        public StatisticPublisherService(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void StatisticMessage(UserStatisticQM message)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_connectionString);

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchangeName = "statistic_event";
            string queueName = "statistic";
            string routingKey = "rout_statistic";

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
