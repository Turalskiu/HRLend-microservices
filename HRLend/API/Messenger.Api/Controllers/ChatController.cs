using Messenger.Api.Attributes;
using Messenger.Api.Domain.Chat;
using Messenger.Api.Domain.DTO.Request;
using Messenger.Api.Domain.DTO.Response.ChatResponse;
using Messenger.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Messenger.Api.Controllers
{
    [Authorize]
    [Route("chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IChatRepository _chatRepository;

        public ChatController(
            IChatRepository chatRepository
            )
        {
            _chatRepository = chatRepository;
        }


        /// <summary>
        /// Создать чат
        /// </summary>
        /// <remarks>
        /// В блок users нужно добавить информация обо всех
        /// участниках чата, в том числе об создатели (его id установить в -1).
        /// </remarks>
        [HttpPost]
        [Route("create")]
        [SwaggerResponse(200, "Успешный запрос", typeof(Chat))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        [SwaggerResponse(500, "Не удалось создать чат")]
        public async Task<ActionResult> CreateChat(ChatCreateRequest model)
        {
            var userId = ((Messenger.Api.Domain.Auth.User)ControllerContext.HttpContext.Items["User"]).Id;

            Chat chat = new Chat
            {
                Title = model.Title,
                Creator = new Creator
                {
                    CreatorId = userId
                },
                Users = model.Users,
                Messages = new List<Message>()
            };

            foreach(var user in model.Users)
            {
                if(user.UserId == -1)
                {
                    user.UserId = userId;
                    break;
                } 
            }

            chat.Messages.Add(new Message
            {
                Guid = Guid.NewGuid().ToString(),
                IsSystem = true,
                DateCreated = DateTime.Now,
                Text = "Чат создан"
            });

            string linkChad = await _chatRepository.InsertChat(chat);
            chat.Id = linkChad;

            return Ok(chat);
        }



        /// <summary>
        /// Возвращает список чатов пользователя
        /// </summary>
        [HttpGet]
        [Route("my")]
        [SwaggerResponse(200, "Успешный запрос", typeof(ListChatResponse))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetChats()
        {
            var userId = ((Messenger.Api.Domain.Auth.User)ControllerContext.HttpContext.Items["User"]).Id;

            List<Chat> chats = await _chatRepository.SelectChat(userId);

            ListChatResponse result = new ListChatResponse
            {
                Chats = chats.Select(c => new ChatShortResponse
                {
                    Id = c.Id,
                    Title = c.Title
                }).ToList()
            };

            return Ok(result);
        }


        /// <summary>
        /// Возвращает список сообщений 
        /// </summary>
        [HttpGet]
        [Route("messages")]
        [SwaggerResponse(200, "Успешный запрос", typeof(List<Message>))]
        [SwaggerResponse(401, "Не авторизован")]
        [SwaggerResponse(403, "Нет прав")]
        public async Task<ActionResult> GetMesages(string chat_id, int skip, int take)
        {
            List<Message> messages = await _chatRepository.SelectMessageChat(chat_id, skip, take);
            return Ok(messages);
        }



        //[HttpGet]
        //[Route("test-mongodb")]
        //public async Task<ActionResult> Test(int id)
        //{
        //    return Ok(await _chatRepository.DeleteUserChat("65c4ff32e943b9bc7cb42bd8", id));
        //}
    }
}
