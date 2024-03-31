using Helpers.Db.Postgres;
using AuthorizationApi.Models;
using AuthorizationApi.Models.DTO;
using System.Data;

namespace AuthorizationApi.Repository
{

    public interface ICabinetRepository
    {
        Cabinet? GetCabinet(int id);
        Cabinet? GetCabinetAndGroupsAndUsers(int id);
        public int InsertCabinet(Cabinet cabinet);
        void UpdateCabinet(Cabinet cabinet);
        void UpdateCabinetStatus(Cabinet cabinet);
        public void DeleteCabinet(int id);


        Group? GetGroup(int cabinetId, int groupId);
        Group? GetGroupAndUsers(int cabinetId, int groupId);
        IEnumerableWithPage<Group> SelectGroups(int cabinetId, Page page);
        IEnumerableWithPage<Group> SelectGroupsByUser(int userId, Page page);
        int InsertGroup(Group group);
        void UpdateGroup(Group group);
        void DeleteGroup(int cabinetId, int groupId);


        bool IsIncludetUserToCabinet(int userId, int cabinetId);
        bool IsIncludetUserToCabinet(string username, int cabinetId);
        User? GetUserInfo(int cabinetId, int userId);
        IEnumerableWithPage<User> SelectUsers(int cabinetId, Page page);
        IEnumerableWithPage<User> SelectUsersByGroup(int cabinetId, int groupId, Page page);


        void ConnectionGroupAndUser(int userId, int  groupId);
        void ConnectionGroupAndUsers(int[] users, int groupId);
        void DeleteConnectionGroupAndUser(int userId, int groupId);


        RegistrationToken? GetRegistrationToken(int id);
        RegistrationToken? GetRegistrationToken(string token);
        IEnumerableWithPage<RegistrationToken> SelectRegistrationToken(int userId, Page page);
        bool InsertRegistrationToken(RegistrationToken token);
        void DeleteRegistrationToken(int id);
        void DeleteRegistrationTokens(int[] id);
    }


    public class CabinetRepository : ICabinetRepository
    {
        private readonly string _connectionString;

        public CabinetRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Cabinet? GetCabinet(int id)
        {

            Func<IDataRecord, Cabinet> convertCabinet = (record) =>
            {
                var entity = new Cabinet
                {
                    Id = id,
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

                return entity;
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };


            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelect<Cabinet>(
                "select * from auth.cabinet__get(" + queryParam + ");",
                convertCabinet,
                parames
            ).FirstOrDefault();
        }
        public Cabinet? GetCabinetAndGroupsAndUsers(int id)
        {
            var converters = new List<Action<IDataRecord, IList<Cabinet>>>();

            Action<IDataRecord, IList<Cabinet>> empty = (record, list) => { };

            Action<IDataRecord, IList<Cabinet>> convertCabinet = (record, list) =>
            {
                var entity = new Cabinet
                {
                    Id = id,
                    StatusId = record.Get<int>("status_id"),
                    Status = new CabinetStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Title = record.Get<string>("title"),
                    Description = record.GetN<string?>("description"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateDelete = record.GetN<DateTime?>("date_delete"),
                    Groups = new List<Group>(),
                    Users = new List<User>()
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<Cabinet>> convertGroup = (record, list) =>
            {
                var entity = new Group
                {
                    Id = record.Get<int>("id"),
                    CabinetId = id,
                    TypeId = record.Get<int>("type_id"),
                    Type = new GroupType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    Title = record.Get<string>("title")
                };


                Cabinet cab = list.FirstOrDefault();
                if (cab != null)
                {
                    cab.Groups.Add(entity);
                }
            };

            Action<IDataRecord, IList<Cabinet>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = record.Get<int>("id"),
                    Username = record.Get<string>("username"),
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo")
                };

                Cabinet cab = list.FirstOrDefault();
                if (cab != null)
                {
                    cab.Users.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", id)
            };

            converters.Add(empty);
            converters.Add(convertCabinet);
            converters.Add(convertGroup);
            converters.Add(convertUser);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectMany<Cabinet>(
                "select * from auth.cabinet_and_group_and_user__get('ref1', 'ref2', 'ref3'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;" +
                "fetch all in ref3;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public int InsertCabinet(Cabinet cabinet)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@StatusId", cabinet.StatusId),
                new KeyValuePair<string, object>("@Title", cabinet.Title),
                new KeyValuePair<string, object>("@Description", cabinet.Description ?? string.Empty),
                new KeyValuePair<string, object>("@DateCreate", cabinet.DateCreate),
                new KeyValuePair<string, object>("@IdCabinet", 0)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<int>(
                "call auth.cabinet__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateCabinet(Cabinet cabinet)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", cabinet.Id),
                new KeyValuePair<string, object>("@Title", cabinet.Title),
                new KeyValuePair<string, object>("@Description", cabinet.Description)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.cabinet__update(" + queryParam + ")",
                parames
            );
        }
        public void UpdateCabinetStatus(Cabinet cabinet)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", cabinet.Id),
                new KeyValuePair<string, object>("@StatusId", cabinet.StatusId),
                new KeyValuePair<string, object>("@DateDelete", cabinet.DateDelete)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.cabinet__update_status(" + queryParam + ")",
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
                "call auth.cabinet__delete(" + queryParam + ")",
                parames
            );
        }


        public Group? GetGroup(int cabinetId, int groupId)
        {

            Func<IDataRecord, Group> convertGroup = (record) =>
            {
                var entity = new Group
                {
                    Id = groupId,
                    CabinetId = cabinetId,
                    TypeId = record.Get<int>("type_id"),
                    Type = new GroupType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    Title = record.Get<string>("title")
                };

                return entity;
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@GroupId", groupId),
            };


            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelect<Group>(
                "select * from auth.group__get(" + queryParam + ");",
                convertGroup,
                parames
            ).FirstOrDefault();
        }
        public Group? GetGroupAndUsers(int cabinetId, int groupId)
        {
            var converters = new List<Action<IDataRecord, IList<Group>>>();

            Action<IDataRecord, IList<Group>> empty = (record, list) => { };

            Action<IDataRecord, IList<Group>> convertGroup = (record, list) =>
            {
                var entity = new Group
                {
                    Id = groupId,
                    CabinetId = cabinetId,
                    TypeId = record.Get<int>("type_id"),
                    Type = new GroupType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    Title = record.Get<string>("title")
                };

                list.Add(entity);
            };

            Action<IDataRecord, IList<Group>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = record.Get<int>("id"),
                    Username = record.Get<string>("username"),
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo")
                };


                Group group = list.FirstOrDefault();
                if (group != null)
                {
                    group.Users.Add(entity);
                }
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@GroupId", groupId),
            };

            converters.Add(empty);
            converters.Add(convertGroup);
            converters.Add(convertUser);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectMany<Group>(
                "select * from auth.group_and_user__get('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<Group> SelectGroups(int cabinetId, Page page)
        {
            var converters = new List<Action<IDataRecord, IList<Group>>>();

            Action<IDataRecord, IList<Group>> empty = (record, list) => { };

            Action<IDataRecord, IList<Group>> convertGroup = (record, list) =>
            {
                var entity = new Group
                {
                    Id = record.Get<int>("id"),
                    CabinetId = cabinetId,
                    TypeId = record.Get<int>("type_id"),
                    Type = new GroupType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    Title = record.Get<string>("title")
                };

                list.Add(entity);
            };


            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@PageNumber", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };


            converters.Add(empty);
            converters.Add(convertGroup);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<Group>(
                "select * from auth.group__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            );
        }
        public IEnumerableWithPage<Group> SelectGroupsByUser(int userId, Page page)
        {
            var converters = new List<Action<IDataRecord, IList<Group>>>();

            Action<IDataRecord, IList<Group>> empty = (record, list) => { };

            Action<IDataRecord, IList<Group>> convertGroup = (record, list) =>
            {
                var entity = new Group
                {
                    Id = record.Get<int>("id"),
                    TypeId = record.Get<int>("type_id"),
                    Type = new GroupType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    Title = record.Get<string>("title")
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
            converters.Add(convertGroup);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<Group>(
                "select * from auth.group__select_by_user('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            );
        }
        public int InsertGroup(Group group)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@CabinetId", group.CabinetId),
                new KeyValuePair<string, object>("@TypeId", group.TypeId),
                new KeyValuePair<string, object>("@Title", group.Title),
                new KeyValuePair<string, object>("@IdGroup", 0),
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<int>(
                "call auth.group__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateGroup(Group group)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@GroupId", group.Id),
                new KeyValuePair<string, object>("@CabinetId", group.CabinetId),
                new KeyValuePair<string, object>("@Title", group.Title)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.group__update(" + queryParam + ")",
                parames
            );
        }
        public void DeleteGroup(int cabinetId, int groupId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@GroupId", groupId),
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.group__delete(" + queryParam + ")",
                parames
            );
        }


        public bool IsIncludetUserToCabinet(int userId, int cabinetId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@UserId", userId),
                new KeyValuePair<string, object>("@CabinetId", cabinetId)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<bool>(
                "select * from auth.user__is_included_cabinet(" + queryParam + ")",
                false,
                parames
            );
        }
        public bool IsIncludetUserToCabinet(string username, int cabinetId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Username", username),
                new KeyValuePair<string, object>("@CabinetId", cabinetId)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<bool>(
                "select * from auth.user__is_included_cabinet(" + queryParam + ")",
                false,
                parames
            );
        }
        public User? GetUserInfo(int cabinetId, int userId)
        {
            var converters = new List<Action<IDataRecord, IList<User>>>();

            Action<IDataRecord, IList<User>> empty = (record, list) => { };

            Action<IDataRecord, IList<User>> convertUser = (record, list) =>
            {
                var entity = new User
                {
                    Id = userId,
                    Username = record.Get<string>("username"),
                    StatusId = record.Get<int>("status_id"),
                    CabinetId = cabinetId,
                    Status = new UserStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Email = record.Get<string>("email"),
                    Photo = record.GetN<string?>("photo"),
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
                new KeyValuePair<string, object>("@cabinetId", cabinetId),
                new KeyValuePair<string, object>("@userId", userId),
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
        public IEnumerableWithPage<User> SelectUsers(int cabinetId, Page page)
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
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@PageNumber", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };


            converters.Add(empty);
            converters.Add(convertUser);
            converters.Add(convertRole);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<User>(
                "select * from auth.user_and_role__select_by_cabinet('ref1', 'ref2', 'ref3'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;" +
                "fetch all in ref3;",
                false,
                parames,
                converters: converters.ToArray()
            );

        }
        public IEnumerableWithPage<User> SelectUsersByGroup(int cabinetId, int groupId, Page page)
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
                new KeyValuePair<string, object>("@CabinetId", cabinetId),
                new KeyValuePair<string, object>("@GroupId", groupId),
                new KeyValuePair<string, object>("@PageNumber", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };


            converters.Add(empty);
            converters.Add(convertUser);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<User>(
                "select * from auth.user__select_by_group('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            );

        }


        public void ConnectionGroupAndUser(int userId, int groupId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@GroupId", groupId),
                new KeyValuePair<string, object>("@UserId", userId)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.group_and_user__insert(" + queryParam + ")",
                parames
            );
        }
        public void ConnectionGroupAndUsers(int[] users, int groupId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@GroupId", groupId),
                new KeyValuePair<string, object>("@UserId", users)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.group_and_user__insert(" + queryParam + ")",
                parames
            );
        }
        public void DeleteConnectionGroupAndUser(int userId, int groupId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@GroupId", groupId),
                new KeyValuePair<string, object>("@UserId", userId)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.group_and_user__delete(" + queryParam + ")",
                parames
            );
        }


        public RegistrationToken? GetRegistrationToken(int id)
        {
            Func<IDataRecord, RegistrationToken> convertRegistrationToken = (record) =>
            {
                var entity = new RegistrationToken
                {
                    UserId = record.Get<int>("user_id"),
                    Token = record.Get<string>("token"),
                    Expires = record.Get<DateTime>("expires"),
                    Created = record.Get<DateTime>("created"),
                    CreatedByIp = record.Get<string>("created_by_ip"),
                    Cabinet = record.Get<int>("cabinet"),
                    CabinetRole = record.Get<int>("cabinet_role")
                };

                return entity;
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelect<RegistrationToken>(
                "select * from auth.registration_token__get(" + queryParam + ");",
                convertRegistrationToken,
                parames
            ).FirstOrDefault();
        }
        public RegistrationToken? GetRegistrationToken(string token)
        {
            Func<IDataRecord, RegistrationToken> convertRegistrationToken = (record) =>
            {
                var entity = new RegistrationToken
                {
                    Id = record.Get<int>("id"),
                    Token = token,
                    Expires = record.Get<DateTime>("expires"),
                    Created = record.Get<DateTime>("created"),
                    CreatedByIp = record.Get<string>("created_by_ip"),
                    Cabinet = record.Get<int>("cabinet"),
                    CabinetRole = record.Get<int>("cabinet_role")
                };

                return entity;
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Token", token)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelect<RegistrationToken>(
                "select * from auth.registration_token__get(" + queryParam + ");",
                convertRegistrationToken,
                parames
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<RegistrationToken> SelectRegistrationToken(int userId, Page page)
        {
            var converters = new List<Action<IDataRecord, IList<RegistrationToken>>>();

            Action<IDataRecord, IList<RegistrationToken>> empty = (record, list) => { };

            Action<IDataRecord, IList<RegistrationToken>> convertRegistrationToken = (record, list) =>
            {
                var entity = new RegistrationToken
                {
                    Id = record.Get<int>("id"),
                    Token = record.Get<string>("token"),
                    Expires = record.Get<DateTime>("expires"),
                    Created = record.Get<DateTime>("created"),
                    CreatedByIp = record.Get<string>("created_by_ip"),
                    Cabinet = record.Get<int>("cabinet"),
                    CabinetRole = record.Get<int>("cabinet_role")
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
            converters.Add(convertRegistrationToken);

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteSelectManyWithPage<RegistrationToken>(
                "select * from auth.registration_token__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters: converters.ToArray()
            );
        }
        public bool InsertRegistrationToken(RegistrationToken token)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@UserId", token.UserId),
                new KeyValuePair<string, object>("@Token", token.Token),
                new KeyValuePair<string, object>("@Expires", token.Expires),
                new KeyValuePair<string, object>("@Created", token.Created),
                new KeyValuePair<string, object>("@CreatedByIp", token.CreatedByIp),
                new KeyValuePair<string, object>("@Cabinet", token.Cabinet),
                new KeyValuePair<string, object>("@CabinetRole", token.CabinetRole),
                new KeyValuePair<string, object>("@IsInsert", true)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            return _connectionString.ExecuteScalar<bool>(
                "call auth.registration_token__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void DeleteRegistrationToken(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.registration_token__delete(" + queryParam + ")",
                parames
            );
        }
        public void DeleteRegistrationTokens(int[] id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@RegistrationTokenId", id)
            };

            string queryParam = PostgresHelper.CreateParameterListString(parames);

            _connectionString.ExecuteNonQuery(
                "call auth.registration_token__delete(" + queryParam + ")",
                parames
            );
        }

    }
}
