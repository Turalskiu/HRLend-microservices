using Helpers.Db.Postgres;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO;
using AuthorizationApi.Models.DTO.Request;
using AuthorizationApi.Services;
using System.Data;

namespace AuthorizationApi.Repository
{

    public interface IAdminRepository
    {
        IEnumerableWithPage<Cabinet> SelectCabinets(Page page);


        IEnumerableWithPage<User> SelectUsers(Page page);
        void DeleteUser(int id);
        void BlockedUser(int userId, string? reasonBlocked, DateTime dateUnblocked);
        void UnblockedUser(int id);


        IEnumerableWithPage<RefreshToken> SelectRefreshTokenByUserId(int userId, Page page);
    }

    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;

        public AdminRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public IEnumerableWithPage<Cabinet> SelectCabinets(Page page)
        {
            var converters = new List<Action<IDataRecord, IList<Cabinet>>>();

            Action<IDataRecord, IList<Cabinet>> empty = (record, list) => { };

            Action<IDataRecord, IList<Cabinet>> convertCabinet = (record, list) =>
            {
                var entity = new Cabinet
                {
                    Id = record.Get<int>("id"),
                    StatusId = record.Get<int>("status_id"),
                    Status = new CabinetStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Title = record.Get<string>("title"),
                    Description = record.GetN<string?>("description"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateDelete = record.GetN<DateTime?>("date_delete")
                };

                list.Add(entity);
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@PageNumber", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };


            converters.Add(empty);
            converters.Add(convertCabinet);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<Cabinet>(
                "select * from auth.cabinet__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            );
        }


        public IEnumerableWithPage<User> SelectUsers(Page page)
        {
            var converters = new List<Action<IDataRecord, IList<User>>>();

            Action<IDataRecord, IList<User>> empty = (record, list) => { };

            Action<IDataRecord, IList<User>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = record.Get<int>("id"),
                    CabinetId = record.Get<int>("cabinet_id"),
                    Username = record.Get<string>("username"),
                    StatusId = record.Get<int>("status_id"),
                    Status = new UserStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo"),
                    Roles = new List<Role>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<User>> convertRole = (record, list) =>
            {
                var entity = new Role
                {
                    Id = record.Get<int>("role_id"),
                    Title = record.Get<string>("role_title")
                };

                int userId = record.Get<int>("user_id");
                User user = list.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Roles.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@PageNumber", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };


            converters.Add(empty);
            converters.Add(convertUser);
            converters.Add(convertRole);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<User>(
                "select * from auth.user_and_role__select('ref1', 'ref2', 'ref3'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;" +
                "fetch all in ref3;",
                false,
                parames,
                converters: converters.ToArray()
            );

        }
        public void DeleteUser(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__delete(" + queryParam + ")",
                parames
            );
        }
        public void BlockedUser(int userId, string? reasonBlocked, DateTime dateUnblocked)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", userId),
                new KeyValuePair<string, object>("@StatusId", (int)USER_STATUS.BLOCKED),
                new KeyValuePair<string, object>("@DateBlocked", DateTime.Now),
                new KeyValuePair<string, object>("@DateUnblocked", dateUnblocked),
                new KeyValuePair<string, object>("@ReasonBlocked", reasonBlocked ?? string.Empty)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__update_status_blocked(" + queryParam + ")",
                parames
            );
        }
        public void UnblockedUser(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@StatusId", (int)USER_STATUS.ACTIVATED),
                new KeyValuePair<string, object>("@DateUnblocked", DateTime.Now)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__update_status_unblocked(" + queryParam + ")",
                parames
            );
        }


        public IEnumerableWithPage<RefreshToken> SelectRefreshTokenByUserId(int userId, Page page)
        {
            var converters = new List<Action<IDataRecord, IList<RefreshToken>>>();

            Action<IDataRecord, IList<RefreshToken>> empty = (record, list) => { };

            Action<IDataRecord, IList<RefreshToken>> convertRefreshToken = (record, list) =>
            {
                var entity = new RefreshToken
                {
                    Id = record.Get<int>("id"),
                    UserId = userId,
                    Token = record.Get<string>("token"),
                    Expires = record.Get<DateTime>("expires"),
                    Created = record.Get<DateTime>("created"),
                    CreatedByIp = record.Get<string>("created_by_ip"),
                    Revoked = record.GetN<DateTime?>("revoked"),
                    RevokedByIp = record.GetN<string?>("revoked_by_ip"),
                    ReplacedByToken = record.GetN<string?>("replaced_by_token"),
                    ReasonRevoked = record.GetN<string?>("reason_revoked")
                };

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@UserId", userId),
                new KeyValuePair<string, object>("@PageNumber", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };

            converters.Add(empty);
            converters.Add(convertRefreshToken);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<RefreshToken>(
                "select * from auth.refresh_token__select_by_user('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            );
        }

    }
}
