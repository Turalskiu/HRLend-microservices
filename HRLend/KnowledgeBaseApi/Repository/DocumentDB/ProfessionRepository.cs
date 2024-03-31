using KnowledgeBaseApi.Domain;
using KnowledgeBaseApi.Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace KnowledgeBaseApi.Repository.DocumentDB
{
    public interface IProfessionRepository
    {
        Task<Profession?> GetProfession(string id);
        Task<List<Profession>> SelectProfession(int skip, int limit, string sort);

        Task<Competence?> GetCompetence(string professionId, string competenceTitle);

        Task<Skill?> GetSkill(string professionId, string competenceTitle, string skillTitle);
    }


    public class ProfessionRepository : IProfessionRepository
    {
        private readonly string _connectionString;
        private readonly string _db;

        public ProfessionRepository(string connectionString, string db)
        {
            _connectionString = connectionString;
            _db = db;
        }

        public async Task<List<Profession>> SelectProfession(int skip, int limit, string sort)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Profession>("profession");

            SortDefinition<Profession?> ssort;
            if(sort == "asc")
                ssort = Builders<Profession>.Sort.Ascending("title");
            else
                ssort = Builders<Profession>.Sort.Descending("title");

            return await collection.Find<Profession>(Builders<Profession>.Filter.Empty)
                .Sort(ssort)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
        }


        public async Task<Profession?> GetProfession(string id)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Profession>("profession");

            var filter = Builders<Profession>.Filter.Eq("_id", ObjectId.Parse(id));
            var document = await collection.Find(filter).FirstOrDefaultAsync();
            return document;
        }


        public async Task<Competence?> GetCompetence(string professionId, string competenceTitle)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Profession>("profession");

            var filter = Builders<Profession>.Filter.Eq("_id", ObjectId.Parse(professionId));
            var profession = await collection.Find(filter).FirstOrDefaultAsync();

            if (profession != null)
            {
                Competence? comp = profession.Competencies.FirstOrDefault(c => c.Title == competenceTitle);
                return comp;
            }

            return null;
        }



        public async Task<Skill?> GetSkill(string professionId, string competenceTitle, string skillTitle)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_db);
            var collection = database.GetCollection<Profession>("profession");

            var filter = Builders<Profession>.Filter.Eq("_id", ObjectId.Parse(professionId));
            var profession= await collection.Find(filter).FirstOrDefaultAsync();

            if(profession != null)
            {
                Competence? comp = profession.Competencies.FirstOrDefault(c=>c.Title == competenceTitle);
                if (comp != null)
                {
                    Skill? skill = comp.Skills.FirstOrDefault(s=>s.Title == skillTitle);
                    return skill;
                }
            }

            return null;
        }
    }
}
