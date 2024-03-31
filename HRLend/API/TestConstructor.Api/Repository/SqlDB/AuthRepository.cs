using HRApi.Domain;
using Helpers.Db.Postgres;


namespace HRApi.Repository.SqlDB
{

    public interface IAuthRepository
    {
        void InsertCabinet(Cabinet cabinet);
        void DeleteCabinet(int id);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertCabinet(Cabinet cabinet)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", cabinet.Id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call hr.cabinet__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void DeleteCabinet(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call hr.cabinet__delete(" + queryParam + ")",
                parames
            );
        }
    }
}
