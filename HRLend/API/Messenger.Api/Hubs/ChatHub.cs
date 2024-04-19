using Messenger.Api.Attributes;
using Messenger.Api.Hubs.Models;
using Messenger.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.Api.SignalRHubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IChatRepository _chatRepository;

        public ChatHub(IDictionary<string, UserConnection> connections,
                IChatRepository chatRepository
            )
        {
            _connections = connections;
            _chatRepository = chatRepository;
        }


        //выйти с диалога
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }


        //присоединиться к диалогу
        public async Task ConnectionChat(UserConnection userConnection)
        {
            var userId = ((Messenger.Api.Domain.Auth.User)Context.Items["User"]).Id;
            userConnection.UserId = userId;

            if(await _chatRepository.IsUserBelongChat(userConnection.ChatLink, userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatLink);
                _connections[Context.ConnectionId] = userConnection;
            }
        }


        //отправить сообщение
        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                string guid = Guid.NewGuid().ToString();

                UserMessage mes = new UserMessage
                {
                    Guid = guid,
                    Message = message,
                    CreateDate = DateTime.Now
                };

                _chatRepository.InsertMessageChat(userConnection.ChatLink, new Domain.Chat.Message
                {
                    Guid = guid,
                    IsSystem = false,
                    User = new Domain.Chat.User
                    {
                        UserId = userConnection.UserId,
                        Username = userConnection.Username,
                        Photo = userConnection.UserPhoto
                    },
                    DateCreated = mes.CreateDate,
                    Text = message
                });

                await Clients.Group(userConnection.ChatLink).SendAsync("ReceiveMessage", userConnection, mes);
            }
        }


        //отправить системное сообщение
        public async Task SendSystemMessage(
            UserConnection userConnection,
            SYSTEM_MESSAGE_TYPE type,
            string message
            )
        {

            SystemMessage mes = new SystemMessage
            {
                Type = type,
                Message = message,
                CreateDate = DateTime.Now
            };

            if(type != SYSTEM_MESSAGE_TYPE.DELETE_CHAT
                && type != SYSTEM_MESSAGE_TYPE.DELETE_MESSAGE)
            {
                _chatRepository.InsertMessageChat(userConnection.ChatLink, new Domain.Chat.Message
                {
                    Guid = null,
                    IsSystem = true,
                    User = null,
                    DateCreated = mes.CreateDate,
                    Text = message
                });
            }


            await Clients.Group(userConnection.ChatLink).SendAsync("ReceiveSystemMessage", userConnection, mes);

        }


        //показывает всех пользователей чата
        public Task SendUsersInChat(string chatLink)
        {
            var users = _chatRepository.SelectUserChat(chatLink);
            return Clients.Group(chatLink).SendAsync("UsersInChat", users);
        }

    }
}
