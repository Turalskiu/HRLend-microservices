using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using StatisticApi.Repository.DocumentDB;
using System.Text;
using Contracts.Test.Queue;
using System.Text.Json;
using StatisticApi.Domain;


namespace StatisticApi.Services.Queue.Consumer
{
    public class StatisticConsumerService : BackgroundService
    {
        private readonly IStatisticRepository _staticRepository;
        private readonly ILogger<StatisticConsumerService> _logger;
        private IConnection _connection;
        private IModel _channel;

        private string exchangeName = "statistic_event";
        private string queueName= "statistic";
        private string routingKey = "rout_statistic";

        public StatisticConsumerService(
            string connectionString,
            IStatisticRepository staticRepository,
            ILogger<StatisticConsumerService> logger)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(connectionString);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
            _channel.BasicQos(0, 1, false);

            _logger = logger;
            _staticRepository = staticRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var consumerStatistic = new EventingBasicConsumer(_channel);
            consumerStatistic.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                UserStatisticQM statistic = JsonSerializer.Deserialize<UserStatisticQM>(message);

                if (statistic is not null)
                {
                    HandleMessage(statistic);
                }

                _channel.BasicAck(ea.DeliveryTag, true);
            };

            _channel.BasicConsume(queue: queueName, consumer: consumerStatistic);

            return Task.CompletedTask;
        }


        private async Task HandleMessage(UserStatisticQM message)
        {
            if (message.MessageType == (int)USER_STATISTIC_TYPE.TEST)
            {
                var statistic = await _staticRepository.GetStatistic(message.User.UserId);

                var stat = new UserStatistic
                {
                    User = new User
                    {
                        UserId = message.User.UserId,
                        Email = message.User.Email,
                        Username = message.User.Username
                    },
                    Competencies = message.Competencies.Select(c => new Competency
                    {
                        TestId = c.TestId,
                        TestTitle = c.TestTitle,
                        Title = c.Title,
                        DateCreate = c.DateCreate,
                        Percent = c.Percent
                    }).ToList(),
                    Skills = message.Skills.Select(c => new Skill
                    {
                        TestId = c.TestId,
                        TestTitle = c.TestTitle,
                        Title = c.Title,
                        DateCreate = c.DateCreate,
                        Percent = c.Percent
                    }).ToList()
                };

                if (statistic is not null)
                    await _staticRepository.UpdateStatisticCompetenceAndSkill(stat);
  
                else
                    await _staticRepository.InsertStatistic(stat);

            }
        }
    }
}
