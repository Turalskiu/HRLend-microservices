using Messenger.Api.Domain.Chat;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


namespace Messenger.Api.Repository
{
    public interface IChatRepository
    {
        Task<string> InsertChat(Chat chat);  //ok
        Task<bool> UpdateTitleChat(string chatId, string newTitle); //ok
        Task<bool> DeleteChat(string chatId);   //ok
        Task<List<Chat>> SelectChat(int userId);  //ok

        bool InsertMessageChat(string chatId, Message message);  //ok
        bool DeleteMessageChat(string chatId, string guid);  //ok
        Task<List<Message>> SelectMessageChat(string chatId, int skip, int take);  //no

        Task<bool> InsertUserChat(string chatId, User user);  //ok
        Task<bool> DeleteUserChat(string chatId, int userId);  //ok
        Task<List<User>> SelectUserChat(string chatId);  //ok
        Task<bool> IsUserBelongChat(string chatId, int userId);  //ok
        Task<bool> IsCreator(string chatId, int userId);  //ok
    }


    public class ChatRepository : IChatRepository
    {
        private readonly string _connectionString;
        private readonly string _db;

        public ChatRepository(string connectionString, string db)
        {
            _connectionString = connectionString;
            _db = db;
        }

        public async Task<string> InsertChat(Chat chat)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            try
            {
                chat.Id = ObjectId.GenerateNewId().ToString();
                await collection.InsertOneAsync(chat);
                return chat.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> UpdateTitleChat(string chatId, string newTitle)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId));
            var update = Builders<Chat>.Update.Set("title", newTitle);
            var result = collection.UpdateOne(filter, update);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteChat(string chatId)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filterBuilder = Builders<Chat>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", ObjectId.Parse(chatId))
            );

            var result = await collection.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<Chat>> SelectChat(int userId)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.ElemMatch("users", Builders<Chat>.Filter.Eq("user_id", userId));
            var projection = Builders<Chat>.Projection.Include("_id").Include("title");
            var documents = await collection.Find(filter).Project<Chat>(projection).ToListAsync();

            return documents;
        }

        public bool InsertMessageChat(string chatId, Message message)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId));
            var update = Builders<Chat>.Update.Push("messages", BsonDocument.Parse(message.ToJson()));
            var result = collection.UpdateOne(filter, update);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteMessageChat(string chatId, string guid)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId)),
                Builders<Chat>.Filter.ElemMatch("messages", Builders<Message>.Filter.Eq("guid", guid))
            );

            var update = Builders<Chat>.Update.PullFilter("messages", Builders<Message>.Filter.Eq("guid", guid));
            var result = collection.UpdateOne(filter, update);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<Message>> SelectMessageChat(string chatId, int skip, int take)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId));
            var sort = Builders<Chat>.Sort.Descending("messages.date_create");
            var projectionMessage = Builders<Chat>.Projection.Include("messages");
            var projectionSkipAndTake = Builders<Chat>.Projection.Slice("messages", skip, take);


            var result = collection.Find<Chat>(filter)
                //.Project(c => c.Messages)
                .Project(projectionMessage)
                .Sort(sort)
                //.SortByDescending()
                .Project(projectionSkipAndTake)      
                .FirstOrDefault();


            if (result != null && result.Contains("messages"))
            {

                var messageList = new List<Message>();
                var messages = result["messages"].AsBsonArray;
                foreach (var m in messages)
                {
                    messageList.Add(BsonSerializer.Deserialize<Message>(m.AsBsonDocument));
                }
                return messageList;
            }

            return null;
        }

        public async Task<bool> InsertUserChat(string chatId, User user)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId));
            var update = Builders<Chat>.Update.Push("users", BsonDocument.Parse(user.ToJson()));
            var result = collection.UpdateOne(filter, update);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteUserChat(string chatId, int userId)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId)),
                Builders<Chat>.Filter.ElemMatch("users", Builders<User>.Filter.Eq("user_id", userId))
            );

            var update = Builders<Chat>.Update.PullFilter("users", Builders<User>.Filter.Eq("user_id", userId));
            var result = collection.UpdateOne(filter, update);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<User>> SelectUserChat(string chatId)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId));
            var projection = Builders<Chat>.Projection.Include("users");
            var document = collection.Find(filter).Project(projection).FirstOrDefault();

            if (document != null && document.Contains("users"))
            {
                var userList = new List<User>();
                var users = document["users"].AsBsonArray;
                foreach (var user in users)
                {
                    userList.Add(BsonSerializer.Deserialize<User>(user.AsBsonDocument));
                }
                return userList;
            }

            return null;
        }
        public async Task<bool> IsUserBelongChat(string chatId, int userId)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId)),
                Builders<Chat>.Filter.ElemMatch("users", Builders<User>.Filter.Eq("user_id", userId))
            );
            var projection = Builders<Chat>.Projection.Include("users");

            var document = collection
                .Find(filter)
                .Project<Chat>(projection)
                .FirstOrDefault();

            if (document != null && document.Users != null)
            {
                var users = document.Users;
                foreach (var u in users)
                {
                    if (u.UserId == userId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public async Task<bool> IsCreator(string chatId, int userId)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Chat>("chat");

            var filter = Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq("_id", ObjectId.Parse(chatId)),
                Builders<Chat>.Filter.Eq("creator.creator_id", userId)
            );

            var document = collection.Find(filter).FirstOrDefault();
            return document != null;
        }
    }
}

