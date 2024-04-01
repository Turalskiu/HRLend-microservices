using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TestApi.Repository.SqlDB;
using TestApi.Domain;
using Contracts.Authorization.Queue;

namespace TestApi.Services.Queue.Consumer
{
    public class AuthConsumerService : BackgroundService
    {

        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthConsumerService> _logger;
        private IConnection _connection;
        private IModel _channel;

        private string exchangeName = "auth_event";
        private string queueNameUser = "user";
        private string routingKeyUser = "rout_user";
        private string queueNameGroup = "group";
        private string routingKeyGroup = "rout_group";

        public AuthConsumerService(
            string connectionString,
            IAuthRepository authRepository,
            ILogger<AuthConsumerService> logger)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(connectionString);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(queue: queueNameUser, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: queueNameGroup, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueNameUser, exchange: exchangeName, routingKey: routingKeyUser);
            _channel.QueueBind(queue: queueNameGroup, exchange: exchangeName, routingKey: routingKeyGroup);
            _channel.BasicQos(0, 1, false);

            _logger = logger;
            _authRepository = authRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var consumerUser = new EventingBasicConsumer(_channel);
            consumerUser.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                UserQM user = JsonSerializer.Deserialize<UserQM>(message);

                if (user is not null)
                {
                    HandleMessage(user);
                }

                _channel.BasicAck(ea.DeliveryTag, true);
            };

            var consumerGroup = new EventingBasicConsumer(_channel);
            consumerGroup.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                GroupQM group = JsonSerializer.Deserialize<GroupQM>(message);

                if (group is not null)
                {
                    HandleMessage(group);
                }
                _channel.BasicAck(ea.DeliveryTag, true);
            };

            _channel.BasicConsume(queue: queueNameUser, consumer: consumerUser);
            _channel.BasicConsume(queue: queueNameGroup, consumer: consumerGroup);

            return Task.CompletedTask;
        }


        private void HandleMessage(UserQM message)
        {
            if (message.MessageType == (int)USER_MESSAGE_TYPE.ADD)
            {
                _authRepository.InsertUser(new Domain.User
                {
                    Id = message.UserId,
                    Email = message.UserEmail,
                    Photo = message.UserPhoto,
                    Username = message.Username
                });
            }
            else if (message.MessageType == (int)USER_MESSAGE_TYPE.UPDATE_PHOTO)
            {
                _authRepository.UpdateUserPhoto(message.UserId, message.UserPhoto);
            }
            else if (message.MessageType == (int)USER_MESSAGE_TYPE.UPDATE_USERNAME)
            {
                _authRepository.UpdateUserUsername(message.UserId, message.Username);
            }

        }


        private void HandleMessage(GroupQM message)
        {
            if (message.MessageType == (int)GROUP_MESSAGE_TYPE.ADD)
            {
                _authRepository.InsertGroup(new Group
                {
                    Id = message.GroupId,
                    Title = message.GroupTitle
                });
            }
            else if (message.MessageType == (int)GROUP_MESSAGE_TYPE.DELETE)
            {
                _authRepository.DeleteGroup(message.GroupId);
            }
            else if (message.MessageType == (int)GROUP_MESSAGE_TYPE.UPDATE_TITLE)
            {
                _authRepository.UpdateGroup(new Group
                {
                    Id = message.GroupId,
                    Title = message.GroupTitle
                });
            }
        }
    }
}
