using Helpers.Db.Postgres;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO;
using System.Data;

namespace AuthorizationApi.Repository
{

    public interface IUserRepository
    {
        User? GetUserInfo(int id);
        User? GetUser(int id);
        User? GetUser(string username);
        User? GetUserAndRefreshToken(string username);
        User? GetUserByRefreshToken(string refreshToken);
        IEnumerableWithPage<User> SelectUsers(Page page);
        int InsertUser(User user);
        void InsertUserRole(int userId, int roleId);
        void UpdateUserUsername(int id, string username);
        void UpdateUserPhoto(int id, string photo);
        void UpdateUserRole(User user);
        void UpdateUserInfo(int id, UserInfo user);
        void UpdateUserPassword(int id, string password);
        void DeleteUser(int id);
        void DeleteUserRole(int userId, int roleId);
        bool IsExistsUsernameUser(string username);
        bool IsExistsEmailUser(string email);

        void DeleteRefreshToken(int[] id);
        bool InsertRefreshToken(RefreshToken refreshToken);
        void UpdateRefreshToken(RefreshToken refreshToken);
        void UpdateRefreshToken(List<int> ids, RefreshToken refreshToken);
    }


    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public User? GetUserInfo(int id)
        {
            var converters = new List<Action<IDataRecord, IList<User>>>();

            Action<IDataRecord, IList<User>> empty = (record, list) => { };

            Action<IDataRecord, IList<User>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = id,
                    Username = record.Get<string>("username"),
                    StatusId = record.Get<int>("status_id"),
                    CabinetId = record.Get<int>("cabinet_id"),
                    Status = new UserStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateDelete = record.GetN<DateTime?>("date_delete"),
                    DateActivation = record.GetN<DateTime?>("date_activation"),
                    DateBlocked = record.GetN<DateTime?>("date_blocked"),
                    DateUnblocked = record.GetN<DateTime?>("date_unblocked"),
                    ReasonBlocked = record.GetN<string?>("reason_blocked"),
                    Info = new UserInfo
                    {
                        FirstName = record.GetN<string?>("first_name"),
                        MiddleName = record.GetN<string?>("middle_name"),
                        LastName = record.GetN<string?>("last_name"),
                        Age = record.GetN<int?>("age")
                    },
                    Roles = new List<Role>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<User>> convertRole = (record, list) =>
            {
                var entity = new Role
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title")
                };

                User user = list.FirstOrDefault();
                if (user != null)
                {
                    user.Roles.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            converters.Add(empty);
            converters.Add(convertUser);
            converters.Add(convertRole);

            string queryParam = PostgresHelper.CreateParameterListString(parames);


            return _connectionString.ExecuteSelectMany<User>(
                "select * from auth.user_info_and_role__get('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public User? GetUser(int id)
        {
            var converters = new List<Action<IDataRecord, IList<User>>>();

            Action<IDataRecord, IList<User>> empty = (record, list) => { };

            Action<IDataRecord, IList<User>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = id,
                    Username = record.Get<string>("username"),
                    StatusId = record.Get<int>("status_id"),
                    CabinetId = record.Get<int>("cabinet_id"),
                    Status = new UserStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo"),
                    PasswordHash = record.Get<string>("password_hash"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateDelete = record.GetN<DateTime?>("date_delete"),
                    DateActivation = record.GetN<DateTime?>("date_activation"),
                    DateBlocked = record.GetN<DateTime?>("date_blocked"),
                    DateUnblocked = record.GetN<DateTime?>("date_unblocked"),
                    ReasonBlocked = record.GetN<string?>("reason_blocked"),
                    Roles = new List<Role>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<User>> convertRole = (record, list) =>
            {
                var entity = new Role
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title")
                };

                User user = list.FirstOrDefault();
                if (user != null)
                {
                    user.Roles.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            converters.Add(empty);
            converters.Add(convertUser);
            converters.Add(convertRole);

            string queryParam = PostgresHelper.CreateParameterListString(parames);


            return _connectionString.ExecuteSelectMany<User>(
                "select * from auth.user_and_role__get('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public User? GetUser(string username)
        {
            var converters = new List<Action<IDataRecord, IList<User>>>();

            Action<IDataRecord, IList<User>> empty = (record, list) => { };

            Action<IDataRecord, IList<User>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = record.Get<int>("id"),
                    Username = username,
                    StatusId = record.Get<int>("status_id"),
                    CabinetId = record.Get<int>("cabinet_id"),
                    Status = new UserStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo"),
                    PasswordHash = record.Get<string>("password_hash"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateDelete = record.GetN<DateTime?>("date_delete"),
                    DateActivation = record.GetN<DateTime?>("date_activation"),
                    DateBlocked = record.GetN<DateTime?>("date_blocked"),
                    DateUnblocked = record.GetN<DateTime?>("date_unblocked"),
                    ReasonBlocked = record.GetN<string?>("reason_blocked"),
                    Roles = new List<Role>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<User>> convertRole = (record, list) =>
            {
                var entity = new Role
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title")
                };

                User user = list.FirstOrDefault();
                if (user != null)
                {
                    user.Roles.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Username", username)
            };

            converters.Add(empty);
            converters.Add(convertUser);
            converters.Add(convertRole);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectMany<User>(
                "select * from auth.user_and_role__get('ref1', 'ref2', 'ref3'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public User? GetUserAndRefreshToken(string username)
        {
            var converters = new List<Action<IDataRecord, IList<User>>>();

            Action<IDataRecord, IList<User>> empty = (record, list) => { };

            Action<IDataRecord, IList<User>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = record.Get<int>("id"),
                    Username = username,
                    StatusId = record.Get<int>("status_id"),
                    CabinetId = record.Get<int>("cabinet_id"),
                    Status = new UserStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo"),
                    PasswordHash = record.Get<string>("password_hash"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateDelete = record.GetN<DateTime?>("date_delete"),
                    DateActivation = record.GetN<DateTime?>("date_activation"),
                    DateBlocked = record.GetN<DateTime?>("date_blocked"),
                    DateUnblocked = record.GetN<DateTime?>("date_unblocked"),
                    ReasonBlocked = record.GetN<string?>("reason_blocked"),
                    Roles = new List<Role>(),
                    RefreshTokens = new List<RefreshToken>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<User>> convertRole = (record, list) =>
            {
                var entity = new Role
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title")
                };

                User user = list.FirstOrDefault();
                if (user != null)
                {
                    user.Roles.Add(entity);
                }
            };


            Action<IDataRecord, IList<User>> convertRefreshToken = (record, list) =>
            {
                var entity = new RefreshToken
                {
                    Id = record.Get<int>("id"),
                    UserId = record.Get<int>("user_id"),
                    Token = record.Get<string>("token"),
                    Expires = record.Get<DateTime>("expires"),
                    Created = record.Get<DateTime>("created"),
                    CreatedByIp = record.Get<string>("created_by_ip"),
                    Revoked = record.GetN<DateTime?>("revoked"),
                    RevokedByIp = record.GetN<string?>("revoked_by_ip"),
                    ReplacedByToken = record.GetN<string?>("replaced_by_token"),
                    ReasonRevoked = record.GetN<string?>("reason_revoked")
                };

                User user = list.FirstOrDefault();
                if (user != null)
                {
                    user.RefreshTokens.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Username", username)
            };

            converters.Add(empty);
            converters.Add(convertUser);
            converters.Add(convertRole);
            converters.Add(convertRefreshToken);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectMany<User>(
                "select * from auth.user_and_role_and_refresh_token__get('ref1', 'ref2', 'ref3'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;" +
                "fetch all in ref3;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public User? GetUserByRefreshToken(string token)
        {
            var converters = new List<Action<IDataRecord, IList<User>>>();

            Action<IDataRecord, IList<User>> empty = (record, list) => { };

            Action<IDataRecord, IList<User>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = record.Get<int>("id"),
                    Username = record.Get<string>("username"),
                    StatusId = record.Get<int>("status_id"),
                    CabinetId = record.Get<int>("cabinet_id"),
                    Status = new UserStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo"),
                    PasswordHash = record.Get<string>("password_hash"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateDelete = record.GetN<DateTime?>("date_delete"),
                    DateActivation = record.GetN<DateTime?>("date_activation"),
                    DateBlocked = record.GetN<DateTime?>("date_blocked"),
                    DateUnblocked = record.GetN<DateTime?>("date_unblocked"),
                    ReasonBlocked = record.GetN<string?>("reason_blocked"),
                    Roles = new List<Role>(),
                    RefreshTokens = new List<RefreshToken>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<User>> convertRole = (record, list) =>
            {
                var entity = new Role
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title")
                };

                User user = list.FirstOrDefault();
                if (user != null)
                {
                    user.Roles.Add(entity);
                }
            };


            Action<IDataRecord, IList<User>> convertRefreshToken = (record, list) =>
            {
                var entity = new RefreshToken
                {
                    Id = record.Get<int>("id"),
                    UserId = record.Get<int>("user_id"),
                    Token = record.Get<string>("token"),
                    Expires = record.Get<DateTime>("expires"),
                    Created = record.Get<DateTime>("created"),
                    CreatedByIp = record.Get<string>("created_by_ip"),
                    Revoked = record.GetN<DateTime?>("revoked"),
                    RevokedByIp = record.GetN<string?>("revoked_by_ip"),
                    ReplacedByToken = record.GetN<string?>("replaced_by_token"),
                    ReasonRevoked = record.GetN<string?>("reason_revoked")
                };

                User user = list.FirstOrDefault();
                if (user != null)
                {
                    user.RefreshTokens.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Token", token)
            };

            converters.Add(empty);
            converters.Add(convertUser);
            converters.Add(convertRole);
            converters.Add(convertRefreshToken);

            string queryParam = PostgresHelper.CreateParameterListString(parames);


            return _connectionString.ExecuteSelectMany<User>(
                "select * from auth.user_and_role_and_refresh_token__get_by_refresh_token('ref1', 'ref2', 'ref3'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;" +
                "fetch all in ref3;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
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
                    Username = record.Get<string>("username"),
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo")
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
            converters.Add(convertUser);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<User>(
                "select * from auth.user__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            );
        }



        public int InsertUser(User user)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", user.CabinetId),
                new KeyValuePair<string, object>("@Username", user.Username),
                new KeyValuePair<string, object>("@Email", user.Email),
                new KeyValuePair<string, object>("@PasswordHash", user.PasswordHash),
                new KeyValuePair<string, object>("@DateCreate", user.DateCreate),
                new KeyValuePair<string, object>("@DateActivation", user.DateActivation),
                new KeyValuePair<string, object>("@StatusId", user.StatusId),
                new KeyValuePair<string, object>("@UserId", 0),
                new KeyValuePair<string, object>("@RoleId", user.Roles.Select(r=>r.Id).ToArray())
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<int>(
                "call auth.user__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void InsertUserRole(int userId, int roleId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@UserId", userId),
                new KeyValuePair<string, object>("@RoleId", roleId)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__add_role(" + queryParam + ")",
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
                "call auth.user__update_username(" + queryParam + ")",
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
                "call auth.user__update_photo(" + queryParam + ")",
                parames
            );
        }
        public void UpdateUserRole(User user)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", user.Id),
                new KeyValuePair<string, object>("@RoleId", user.Roles.Select(r=>r.Id).ToArray())
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__update_role(" + queryParam + ")",
                parames
            );
        }
        public void UpdateUserInfo(int id, UserInfo user)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@FirstName", user.FirstName),
                new KeyValuePair<string, object>("@MiddleName", user.MiddleName),
                new KeyValuePair<string, object>("@LastName", user.LastName),
                new KeyValuePair<string, object>("@Age", user.Age)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user_info__update(" + queryParam + ")",
                parames
            );
        }
        public void UpdateUserPassword(int id, string password)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@PasswordHash", password),
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__update_password(" + queryParam + ")",
                parames
            );
        }
        public void DeleteUser(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@StatusId", (int)USER_STATUS.DELETED),
                new KeyValuePair<string, object>("@DateDelete", DateTime.Now)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__update_status_delete(" + queryParam + ")",
                parames
            );
        }
        public void DeleteUserRole(int userId, int roleId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@UserId", userId),
                new KeyValuePair<string, object>("@RoleId", roleId)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.user__delete_role(" + queryParam + ")",
                parames
            );
        }
        public bool IsExistsUsernameUser(string username)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Username", username)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<bool>(
                "select * from auth.user__is_exists_username(" + queryParam + ")",
                false,
                parames
            );
        }
        public bool IsExistsEmailUser(string email)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Email", email)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<bool>(
                "select * from auth.user__is_exists_email(" + queryParam + ")",
                false,
                parames
            );
        }


        public bool InsertRefreshToken(RefreshToken refreshToken)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@UserId", refreshToken.UserId),
                new KeyValuePair<string, object>("@Token", refreshToken.Token),
                new KeyValuePair<string, object>("@Expires", refreshToken.Expires),
                new KeyValuePair<string, object>("@Created", refreshToken.Created),
                new KeyValuePair<string, object>("@CreatedByIp", refreshToken.CreatedByIp),
                new KeyValuePair<string, object>("@IsInsert", true)

            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<bool>(
                "call auth.refresh_token__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateRefreshToken(List<int> ids, RefreshToken refreshToken)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@RefreshTokenId", ids),
                new KeyValuePair<string, object>("@Revoked", refreshToken.Revoked),
                new KeyValuePair<string, object>("@RevokedById", refreshToken.RevokedByIp ?? string.Empty),
                new KeyValuePair<string, object>("@ReplacedByToken", refreshToken.ReplacedByToken ?? string.Empty),
                new KeyValuePair<string, object>("@ReasonRevoced", refreshToken.ReasonRevoked ?? string.Empty)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.refresh_token__update(" + queryParam + ")",
                parames
            );
        }
        public void UpdateRefreshToken(RefreshToken refreshToken)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@RefreshTokenId", refreshToken.Id),
                new KeyValuePair<string, object>("@Revoked", refreshToken.Revoked),
                new KeyValuePair<string, object>("@RevokedById", refreshToken.RevokedByIp),
                new KeyValuePair<string, object>("@ReplacedByToken", refreshToken.ReplacedByToken),
                new KeyValuePair<string, object>("@ReasonRevoced", refreshToken.ReasonRevoked),
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.refresh_token__update(" + queryParam + ")",
                parames
            );
        }
        public void DeleteRefreshToken(int[] id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@RefreshTokenId", id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.refresh_token__delete(" + queryParam + ")",
                parames
            );
        }

    }
}
