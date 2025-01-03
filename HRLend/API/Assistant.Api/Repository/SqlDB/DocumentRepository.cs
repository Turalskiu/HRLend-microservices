﻿using Assistant.Api.Domain;
using Helpers.Db.Postgres;
using System.Data;

namespace Assistant.Api.Repository.SqlDB
{
    public interface IDocumentRepository
    {
        int InsertDocument(Document doc);
        void DeleteDocument(int id);
        Document? GetDocument(int id);
        IEnumerable<Document> SelectDocument(int cabinetId);
    }


    public class DocumentRepository : IDocumentRepository
    {
        private readonly string _connectionString;

        public DocumentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int InsertDocument(Document doc)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", doc.CabinetId),
                new KeyValuePair<string, object>("@Title", doc.Title??string.Empty),
                new KeyValuePair<string, object>("@TypeId", doc.Type.Id),
                new KeyValuePair<string, object>("@ElasticsearchIndex", doc.ElasticsearchIndex??string.Empty),
                new KeyValuePair<string, object>("@IdDocument", 0)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call assistant.document__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void DeleteDocument(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call assistant.document__delete(" + queryParam + ")",
                false,
                parames
            );
        }
        public Document? GetDocument(int id)
        {

            Func<IDataRecord, Document> convertDocument = (record) =>
            {
                var entity = new Document
                {
                    Id = id,
                    CabinetId = record.Get<int>("cabinet_id"),
                    Title = record.Get<string>("title"),
                    ElasticsearchIndex = record.Get<string>("elasticsearch_index")

                };
                return entity;
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelect(
                "select * from assistant.document__get(" + queryParam + ");",
                convertDocument,
                parames
            ).FirstOrDefault();
        }
        public IEnumerable<Document> SelectDocument(int cabinetId)
        {

            Func<IDataRecord, Document> convertDocument = (record) =>
            {
                var entity = new Document
                {
                    Id = record.Get<int>("id"),
                    CabinetId = cabinetId,
                    Type = new DocumentType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    Title = record.Get<string>("title"),
                    ElasticsearchIndex = record.Get<string>("elasticsearch_index")
                };
                return entity;
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelect(
                "select * from assistant.document__select(" + queryParam + ");",
                convertDocument,
                parames
            );
        }
    }
}
