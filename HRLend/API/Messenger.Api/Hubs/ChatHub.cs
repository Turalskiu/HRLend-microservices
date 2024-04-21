using Messenger.Api.Attributes;
using Messenger.Api.Domain.Chat;
using Messenger.Api.Hubs.Models;
using Messenger.Api.Hubs.Models.Request;
using Messenger.Api.Hubs.Models.Response;
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

                UserMessageResponse mes = new UserMessageResponse
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
        public async Task SendSystemMessage(SystemMessageRequest request)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection sender))
            {
                SystemMessageResponse response = new SystemMessageResponse();
                response.CreateDate = DateTime.Now;
                response.Type = request.Type;

                if (request.Type == SYSTEM_MESSAGE_TYPE.DELETE_CHAT) {}   //удаление чата
                else if(request.Type == SYSTEM_MESSAGE_TYPE.DELETE_MESSAGE)   //удаления сообщения
                {
                    if(sender.UserId == request.DeleteMessageInfo.CreatorId 
                        || await _chatRepository.IsCreator(sender.ChatLink, sender.UserId))
                    {
                        _chatRepository.DeleteMessageChat(sender.ChatLink, request.DeleteMessageInfo.MessageGuid);

                        response.DeleteMessageInfo = new DeleteMessageInfoResponse
                        {
                            MessageGuid = request.DeleteMessageInfo.MessageGuid
                        };
                    }
                }
                else if(request.Type == SYSTEM_MESSAGE_TYPE.DELETE_USER)  //удалить пользователя из чата
                {
                    if(await _chatRepository.IsCreator(sender.ChatLink, sender.UserId))
                    {
                        _chatRepository.DeleteUserChat(sender.ChatLink, request.DeleteUserInfo.UserId);

                        string message = $"{sender.Username} исключил {request.DeleteUserInfo.Username}";

                        _chatRepository.InsertMessageChat(sender.ChatLink, new Domain.Chat.Message
                        {
                            Guid = null,
                            IsSystem = true,
                            User = null,
                            DateCreated = DateTime.Now,
                            Text = message
                        });

                        response.Message = message;
                    }
                }
                else if(request.Type == SYSTEM_MESSAGE_TYPE.ADD_USER)  //добавить пользователя в чат
                {
                    if(await _chatRepository.IsCreator(sender.ChatLink, sender.UserId))
                    {
                        await _chatRepository.InsertUserChat(sender.ChatLink, new User
                        {
                            UserId = request.AddUserInfo.UserId,
                            Username = request.AddUserInfo.Username,
                            Photo = request.AddUserInfo.Photo
                        });

                        string message = $"{sender.Username} пригласил {request.AddUserInfo.Username}";

                        _chatRepository.InsertMessageChat(sender.ChatLink, new Domain.Chat.Message
                        {
                            Guid = null,
                            IsSystem = true,
                            User = null,
                            DateCreated = DateTime.Now,
                            Text = message
                        });

                        response.Message = message;
                    }
                }
                else if (request.Type == SYSTEM_MESSAGE_TYPE.EXIT_USER)  //пользователь покинул чат
                {
                    _chatRepository.DeleteUserChat(sender.ChatLink, sender.UserId);

                    string message = $"{sender.Username} покинул чат";

                    _chatRepository.InsertMessageChat(sender.ChatLink, new Domain.Chat.Message
                    {
                        Guid = null,
                        IsSystem = true,
                        User = null,
                        DateCreated = DateTime.Now,
                        Text = message
                    });

                    response.Message = message;

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, sender.ChatLink);
                    _connections.Remove(Context.ConnectionId);
                }
                else if (request.Type == SYSTEM_MESSAGE_TYPE.UPDATE_CHAT_TITLE)  //обновить название чата
                {
                    if (await _chatRepository.IsCreator(sender.ChatLink, sender.UserId))
                    {
                        await _chatRepository.UpdateTitleChat(sender.ChatLink, request.UpdateChatTitleInfo.NewTitle);

                        string message = $"{sender.Username} поменял название чата на {request.UpdateChatTitleInfo.NewTitle}";

                        _chatRepository.InsertMessageChat(sender.ChatLink, new Domain.Chat.Message
                        {
                            Guid = null,
                            IsSystem = true,
                            User = null,
                            DateCreated = DateTime.Now,
                            Text = message
                        });

                        response.Message = message;
                        response.UpdateChatTitleInfo = new UpdateChatTitleInfoResponse
                        {
                            NewTitle = request.UpdateChatTitleInfo.NewTitle
                        };
                    }
                }

                await Clients.Group(sender.ChatLink).SendAsync("ReceiveSystemMessage", sender, response);
            }
        }


        //показывает всех пользователей чата
        public Task SendUsersInChat(string chatLink)
        {
            var users = _chatRepository.SelectUserChat(chatLink);
            return Clients.Group(chatLink).SendAsync("UsersInChat", users);
        }

    }
}
