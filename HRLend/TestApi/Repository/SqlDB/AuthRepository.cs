using Helpers.Db.Postgres;
using TestApi.Domain;

namespace TestApi.Repository.SqlDB
{
    public interface IAuthRepository
    {
        void InsertUser(User user);
        void UpdateUserUsername(int id, string username);
        void UpdateUserPhoto(int id, string photo);

        void InsertGroup(Group group);
        void UpdateGroup(Group group);
        void DeleteGroup(int id);
    }


    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertUser(User user)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", user.Id),
                new KeyValuePair<string, object>("@Username", user.Username),
                new KeyValuePair<string, object>("@Photo", user.Photo?? string.Empty),
                new KeyValuePair<string, object>("@Email", user.Email)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call test.user__insert(" + queryParam + ")",
                parames
            );
        }
        public void UpdateUserUsername(int id, string username)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@Username", username)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call test.user__update_username(" + queryParam + ")",
                parames
            );
        }
        public void UpdateUserPhoto(int id, string photo)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@Photo", photo)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call test.user__update_photo(" + queryParam + ")",
                parames
            );
        }

        public void InsertGroup(Group group)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", group.Id),
                new KeyValuePair<string, object>("@Title", group.Title)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call test.group__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateGroup(Group group)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", group.Id),
                new KeyValuePair<string, object>("@Title", group.Title)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call test.group__update(" + queryParam + ")",
                false,
                parames
            );
        }
        public void DeleteGroup(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call test.group__delete(" + queryParam + ")",
                parames
            );
        }
    }
}
