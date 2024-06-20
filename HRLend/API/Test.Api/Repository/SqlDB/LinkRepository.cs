using System.Data;
using TestApi.Domain;
using TestApi.Domain.DTO;
using Helpers.Db.Postgres;

namespace TestApi.Repository.SqlDB
{
    public interface ILinkRepository
    {
        int InsertTestLink(TestLink link);
        void UpdateTestLink(int id, int statusId);
        void ClosedTestLink(int id);
        void IncreaseCandidateCountTestLink(int id);
        void DeleteTestLink(int id);
        TestLink? GetTestLink(int id);
        TestLink? GetTestLink(string link);
        IEnumerableWithPage<TestLink> SelectTestLink(Page page, int hrId);
        bool IsCheckPassingTestLinkByEmail(int linkId, string email);

        int InsertTestLinkResponse(TestLinkResponse response);
        void UpdateTestLinkResponse(int id, int statusId);
        void DeleteTestLinkResponse(int id);
        TestLinkResponse? GetTestLinkResponseAndUser(int id);
        IEnumerableWithPage<TestLinkResponse> SelectTestLinkResponseAndUser(Page page, int testLinkId);
        string? GetTestLinkResponseRowTestResultLink(int id);
        int GetTestLinkResponseCount(int linkId, int userId);

        int InsertTestResult(TestResult result);
        int InsertAnonymousUser(AnonymousUser user);
    }


    public class LinkRepository : ILinkRepository
    {
        private readonly string _connectionString;

        public LinkRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public int InsertTestLink(TestLink link)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@TestId", link.TestId),
                new KeyValuePair<string, object>("@StatusId", link.StatusId),
                new KeyValuePair<string, object>("@TypeId", link.TypeId),
                new KeyValuePair<string, object>("@UserId", link.UserId),
                new KeyValuePair<string, object>("@GroupId", link.GroupId),
                new KeyValuePair<string, object>("@LimitCandidateCountId", link.LimitCandidateCount),
                new KeyValuePair<string, object>("@LimitAttempt", link.LimitAttempt),
                new KeyValuePair<string, object>("@Title", link.Title??string.Empty),
                new KeyValuePair<string, object>("@Link", link.Link??string.Empty),
                new KeyValuePair<string, object>("@DateCreate", DateTime.UtcNow),
                new KeyValuePair<string, object>("@DateExpired", link.DateExpired),
                new KeyValuePair<string, object>("@IdTestLink", 0)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call test.test_link__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateTestLink(int id, int statusId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@StatusId", statusId)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test_link__update(" + queryParam + ")",
                false,
                parames
            );
        }
        public void ClosedTestLink(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@StatusId", (int)TEST_LINK_STATUS.CLOSED),
                new KeyValuePair<string, object>("@DateClosed", DateTime.UtcNow)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test_link__closed(" + queryParam + ")",
                false,
                parames
            );
        }
        public void IncreaseCandidateCountTestLink(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test_link__candidate_count_increase(" + queryParam + ")",
                false,
                parames
            );
        }
        public void DeleteTestLink(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test_link__delete(" + queryParam + ")",
                false,
                parames
            );
        }
        public TestLink? GetTestLink(int id)
        {
            var converters = new List<Action<IDataRecord, IList<TestLink>>>();

            Action<IDataRecord, IList<TestLink>> empty = (record, list) => { };
            Action<IDataRecord, IList<TestLink>> convertTestLink = (record, list) =>
            {
                var entity = new TestLink
                {
                    Id = id,
                    Title = record.Get<string>("title"),
                    Status = new TestLinkStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Type = new TestLinkType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    UserId = record.GetN<int?>("user_id"),
                    GroupId = record.GetN<int?>("group_id"),
                    TestId = record.Get<int>("test_id"),
                    LimitCandidateCount = record.GetN<int?>("limit_candidate_count"),
                    LimitAttempt = record.Get<int>("limit_attempt"),
                    CandidateCount = record.Get<int>("candidate_count"),
                    Link = record.Get<string>("link"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateClosed = record.GetN<DateTime?>("date_closed"),
                    DateExpired = record.Get<DateTime>("date_expired")

                };

                if (entity.UserId != null)
                {
                    entity.Candidate = new User
                    {
                        Id = (int)entity.UserId,
                        Username = record.GetN<string?>("username"),
                        Email = record.GetN<string?>("user_email"),
                        Photo = record.GetN<string?>("user_photo")
                    };
                }
                if(entity.GroupId != null)
                {
                    entity.Group = new Group
                    {
                        Id = (int)entity.GroupId,
                        Title = record.GetN<string?>("group_title")
                    };
                }

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            converters.Add(empty);
            converters.Add(convertTestLink);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from test.test_link__get('ref1'," + queryParam + ");" +
                "fetch all in ref1;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public TestLink? GetTestLink(string link)
        {
            var converters = new List<Action<IDataRecord, IList<TestLink>>>();

            Action<IDataRecord, IList<TestLink>> empty = (record, list) => { };
            Action<IDataRecord, IList<TestLink>> convertTestLink = (record, list) =>
            {
                var entity = new TestLink
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title"),
                    Status = new TestLinkStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    Type = new TestLinkType
                    {
                        Id = record.Get<int>("type_id"),
                        Title = record.Get<string>("type_title")
                    },
                    UserId = record.GetN<int?>("user_id"),
                    GroupId = record.GetN<int?>("group_id"),
                    TestId = record.Get<int>("test_id"),
                    LimitCandidateCount = record.GetN<int?>("limit_candidate_count"),
                    LimitAttempt = record.Get<int>("limit_attempt"),
                    CandidateCount = record.Get<int>("candidate_count"),
                    Link = link,
                    DateCreate = record.Get<DateTime>("date_create"),
                    DateClosed = record.GetN<DateTime?>("date_closed"),
                    DateExpired = record.Get<DateTime>("date_expired")

                };

                if (entity.UserId != null)
                {
                    entity.Candidate = new User
                    {
                        Id = (int)entity.UserId,
                        Username = record.Get<string>("username"),
                        Email = record.Get<string>("user_email"),
                        Photo = record.GetN<string?>("user_photo")
                    };
                }
                if (entity.GroupId != null)
                {
                    entity.Group = new Group
                    {
                        Id = (int)entity.GroupId,
                        Title = record.Get<string>("group_title")
                    };
                }

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Link", link)
            };

            converters.Add(empty);
            converters.Add(convertTestLink);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from test.test_link__get_by_link('ref1'," + queryParam + ");" +
                "fetch all in ref1;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<TestLink> SelectTestLink(Page page, int hrId)
        {
            var converters = new List<Action<IDataRecord, IList<TestLink>>>();

            Action<IDataRecord, IList<TestLink>> empty = (record, list) => { };

            Action<IDataRecord, IList<TestLink>> convertTestLink = (record, list) =>
            {
                var entity = new TestLink
                {
                    Id = record.Get<int>("id"),
                    Title = record.Get<string>("title"),
                    Status = new TestLinkStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    }
                };

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@HrId", hrId),
                new KeyValuePair<string, object>("@PageOn", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };

            converters.Add(empty);
            converters.Add(convertTestLink);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectManyWithPage(
                "select * from test.test_link__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters.ToArray()
            );
        }
        public bool IsCheckPassingTestLinkByEmail(int linkId, string email)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@LinkId", linkId),
                new KeyValuePair<string, object>("@UserEmail", email)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<bool>(
                "select * from test.test_link__is_check_passing_user(" + queryParam + ")",
                false,
                parames
            );
        }


        public void DeleteTestLinkResponse(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test_link_response__delete(" + queryParam + ")",
                false,
                parames
            );
        }
        public int InsertTestLinkResponse(TestLinkResponse response)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@TestLinkId", response.TestLinkId),
                new KeyValuePair<string, object>("@StatusId", response.StatusId),
                new KeyValuePair<string, object>("@UserId", response.UserId),
                new KeyValuePair<string, object>("@NumberAttempt", response.NumberAttempt),
                new KeyValuePair<string, object>("@TestGeneratedLink", response.TestGeneratedLink??string.Empty),
                new KeyValuePair<string, object>("@DateCreate", DateTime.UtcNow),
                new KeyValuePair<string, object>("@IdTestLinkResponse", 0)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call test.test_link_response__insert(" + queryParam + ")",
                false,
                parames
            );
        }
        public void UpdateTestLinkResponse(int id, int statusId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id),
                new KeyValuePair<string, object>("@StatusId", statusId)
            };

            string queryParam = parames.CreateParameterListString();

            _connectionString.ExecuteNonQuery(
                "call test.test_link_response__update(" + queryParam + ")",
                false,
                parames
            );
        }
        public TestLinkResponse? GetTestLinkResponseAndUser(int id)
        {
            var converters = new List<Action<IDataRecord, IList<TestLinkResponse>>>();

            Action<IDataRecord, IList<TestLinkResponse>> empty = (record, list) => { };
            Action<IDataRecord, IList<TestLinkResponse>> convertTestLinkResponse = (record, list) =>
            {
                var entity = new TestLinkResponse
                {
                    Id = id,
                    TestLinkId = record.Get<int>("test_link_id"),
                    TestGeneratedLink = record.GetN<string?>("test_generated_link"),
                    UserId = record.GetN<int?>("user_id"),
                    Status = new TestLinkResponseStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    NumberAttempt = record.Get<int>("number_attempt"),
                    DateCreate = record.Get<DateTime>("date_create")
                };

                if (entity.UserId != null)
                {
                    entity.Candidate = new User
                    {
                        Id = (int)entity.UserId,
                        Username = record.GetN<string?>("username"),
                        Email = record.GetN<string?>("user_email"),
                        Photo = record.GetN<string?>("user_photo")
                    };
                }

                var anonymousUserId = record.GetN<int?>("anonymous_user_id");
                if (anonymousUserId != null)
                {
                    entity.AnonymousCandidate = new AnonymousUser
                    {
                        Id = (int)anonymousUserId,
                        FirstName = record.GetN<string?>("anonymous_user_first_name"),
                        LastName = record.GetN<string?>("anonymous_user_last_name"),
                        MiddleName = record.GetN<string?>("anonymous_user_middle_name"),
                        Email = record.GetN<string?>("anonymous_user_email"),
                    };
                }

                var testResultId = record.GetN<int?>("test_result_id");
                if (testResultId != null)
                {
                    entity.TestResult = new TestResult
                    {
                        Id = (int)testResultId,
                        TestResultLink = record.GetN<string?>("test_result_link"),
                        TestTemplateStatisticsLink = record.Get<string>("test_template_statistics_link"),
                        IsPassed = record.GetN<bool>("test_is_passed")
                    };
                }

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            converters.Add(empty);
            converters.Add(convertTestLinkResponse);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectMany(
                "select * from test.test_link_and_user_response__get('ref1'," + queryParam + ");" +
                "fetch all in ref1;",
                false,
                parames,
                converters: converters.ToArray()
            ).FirstOrDefault();
        }
        public IEnumerableWithPage<TestLinkResponse> SelectTestLinkResponseAndUser(Page page, int testLinkId)
        {
            var converters = new List<Action<IDataRecord, IList<TestLinkResponse>>>();

            Action<IDataRecord, IList<TestLinkResponse>> empty = (record, list) => { };

            Action<IDataRecord, IList<TestLinkResponse>> convertTestLinkResponse = (record, list) =>
            {
                var entity = new TestLinkResponse
                {
                    Id = record.Get<int>("id"),
                    DateCreate = record.Get<DateTime>("date_create"),
                    Status = new TestLinkResponseStatus
                    {
                        Id = record.Get<int>("status_id"),
                        Title = record.Get<string>("status_title")
                    },
                    NumberAttempt = record.Get<int>("number_attempt")
                };

                var userId = record.GetN<int?>("user_id");
                if (userId != null)
                {
                    entity.Candidate = new User
                    {
                        Id = (int)userId,
                        Username = record.GetN<string?>("username"),
                        Email = record.GetN<string?>("user_email"),
                        Photo = record.GetN<string?>("user_photo")
                    };
                }

                var anonymousUserId = record.GetN<int?>("anonymous_user_id");
                if (anonymousUserId != null)
                {
                    entity.AnonymousCandidate = new AnonymousUser
                    {
                        Id = (int)anonymousUserId,
                        FirstName = record.GetN<string?>("anonymous_user_first_name"),
                        Email = record.GetN<string?>("anonymous_user_email"),
                        MiddleName = record.GetN<string?>("anonymous_user_middle_name"),
                        LastName = record.GetN<string?>("anonymous_user_last_name")
                    };
                }

                var testResultId = record.GetN<int?>("test_result_id");
                if (testResultId != null)
                {
                    entity.TestResult = new TestResult
                    {
                        Id = (int)testResultId,
                        TestResultLink = record.GetN<string?>("test_result_link"),
                        TestTemplateStatisticsLink = record.Get<string>("test_template_statistics_link"),
                        IsPassed = record.GetN<bool>("test_is_passed")
                    };
                }

                list.Add(entity);
            };

            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@TestLinkId", testLinkId),
                new KeyValuePair<string, object>("@PageOn", page.PageNumber),
                new KeyValuePair<string, object>("@PageSize", page.PageSize),
                new KeyValuePair<string, object>("@PageSort", page.Sort ?? string.Empty)
            };

            converters.Add(empty);
            converters.Add(convertTestLinkResponse);

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteSelectManyWithPage(
                "select * from test.test_link_response_and_user__select('ref1', 'ref2'," + queryParam + ");" +
                "fetch all in ref1;" +
                "fetch all in ref2;",
                false,
                parames,
                converters.ToArray()
            );
        }
        public string? GetTestLinkResponseRowTestResultLink(int id)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@Id", id)
            };

            string queryParam = parames.CreateParameterListString();

            var result = _connectionString.ExecuteScalar<string>(
                "select * from test.test_link_response__get_test_result_link(" + queryParam + ")",
                false,
                parames
            );

            if (result == "") return null;
            return result;
        }
        public int GetTestLinkResponseCount(int linkId, int userId)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@LinkId", linkId),
                new KeyValuePair<string, object>("@UserId", userId)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "select * from test.test_link_response__count(" + queryParam + ")",
                false,
                parames
            );
        }


        public int InsertAnonymousUser(AnonymousUser user)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@TestLinkResponseId", user.TestLinkResponseId),
                new KeyValuePair<string, object>("@FirstName", user.FirstName??string.Empty),
                new KeyValuePair<string, object>("@LastName", user.LastName??string.Empty),
                new KeyValuePair<string, object>("@MiddleName", user.MiddleName??string.Empty),
                new KeyValuePair<string, object>("@Email", user.Email??string.Empty),
                new KeyValuePair<string, object>("@IdAnonymousUser", 0)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call test.anonymous_user__insert(" + queryParam + ")",
                false,
                parames
            );
        }


        public int InsertTestResult(TestResult result)
        {
            var parames = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("@TestLinkResponseId", result.TestLinkResponseId),
                new KeyValuePair<string, object>("@IsPassed", result.IsPassed),
                new KeyValuePair<string, object>("@TestResultLink", result.TestResultLink),
                new KeyValuePair<string, object>("@TestTemplateStatisticsLink", result.TestTemplateStatisticsLink),
                new KeyValuePair<string, object>("@IdTestResult", 0)
            };

            string queryParam = parames.CreateParameterListString();

            return _connectionString.ExecuteScalar<int>(
                "call test.test_result__insert(" + queryParam + ")",
                false,
                parames
            );
        }
    }
}
