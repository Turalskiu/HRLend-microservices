using StatisticApi.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using StatisticApi.Domain.Auth;

namespace StatisticApi.Repository.DocumentDB
{
    public interface IStatisticRepository
    {
        Task<UserStatistic?> GetStatistic(int userId);
        Task<string> InsertStatistic(UserStatistic statistic);
        Task<bool> UpdateStatisticCompetenceAndSkill(UserStatistic statistic);
        Task<bool> DeleteStatistic(string id);
    }


    public class StatisticRepository : IStatisticRepository
    {
        private readonly string _connectionString;
        private readonly string _db;

        public StatisticRepository(string connectionString, string db)
        {
            _connectionString = connectionString;
            _db = db;
        }

        public async Task<UserStatistic?> GetStatistic(int userId)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<UserStatistic>("user_competence_and_skill_statistic");

            var filter = Builders<UserStatistic>.Filter.Eq(doc => doc.User.UserId, userId);
            var document = await collection.Find(filter).FirstOrDefaultAsync();
            return document;
        }
        public async Task<string> InsertStatistic(UserStatistic statistic)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<UserStatistic>("user_competence_and_skill_statistic");

            try
            {
                statistic.Id = ObjectId.GenerateNewId().ToString();
                await collection.InsertOneAsync(statistic);
                return statistic.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> UpdateStatisticCompetenceAndSkill(UserStatistic statistic)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<UserStatistic>("user_competence_and_skill_statistic");

            var filter = Builders<UserStatistic>.Filter.Eq(doc => doc.User.UserId, statistic.User.UserId);
            var update = Builders<UserStatistic>.Update
                            //.Push("competencies", BsonDocument.Parse(statistic.Competencies.ToJson()))
                            //.Push("skills", BsonDocument.Parse(statistic.Skills.ToJson()));
                            .PushEach(doc => doc.Competencies, statistic.Competencies)
                            .PushEach(doc => doc.Skills, statistic.Skills);
            var result = await collection.UpdateOneAsync(filter, update);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteStatistic(string link)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<UserStatistic>("user_competence_and_skill_statistic");

            var filterBuilder = Builders<UserStatistic>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq("_id", ObjectId.Parse(link))
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
    }
}
