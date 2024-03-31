using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace TestGeneratorApi.Utils
{
    public static class MapUtil
    {
        public static void Init()
        {
            //TestModuleGRPC
            BsonClassMap.RegisterClassMap<Domain.GRPC.TestModuleGRPC.Module>(cm =>
            {
                cm.MapMember(c => c.Id).SetElementName("_id").SetSerializer(new StringSerializer(BsonType.ObjectId));
                cm.MapMember(c => c.Title).SetElementName("title");
                cm.MapMember(c => c.Options).SetElementName("options");
                cm.MapMember(c => c.Rule).SetElementName("rule");
                cm.MapMember(c => c.QuestionsList).SetElementName("questions");
                cm.MapMember(c => c.RecommendationsList).SetElementName("recommendations");
            });
            BsonClassMap.RegisterClassMap<Domain.GRPC.TestModuleGRPC.Options>(cm =>
            {
                cm.MapMember(c => c.IsDefault).SetElementName("is_default");
                cm.MapMember(c => c.CountQuestions).SetElementName("count_questions");
                cm.MapMember(c => c.TakeQuestions).SetElementName("take_questions");
                cm.MapMember(c => c.LimitDurationInSeconds).SetElementName("limit_duration_in_seconds");
            });
            BsonClassMap.RegisterClassMap<Domain.GRPC.TestModuleGRPC.Rule>(cm =>
            {
                cm.MapMember(c => c.MinValueForPassed).SetElementName("min_value_for_passed");
            });
            BsonClassMap.RegisterClassMap<Domain.GRPC.TestModuleGRPC.Question>(cm =>
            {
                cm.MapMember(c => c.Id).SetElementName("id");
                cm.MapMember(c => c.Text).SetElementName("text");
                cm.MapMember(c => c.Description).SetElementName("description");
                cm.MapMember(c => c.Type).SetElementName("type");
                cm.MapMember(c => c.MaxValue).SetElementName("max_value");
                cm.MapMember(c => c.OptionsList).SetElementName("options");
            });
            BsonClassMap.RegisterClassMap<Domain.GRPC.TestModuleGRPC.Option>(cm =>
            {
                cm.MapMember(c => c.Id).SetElementName("id");
                cm.MapMember(c => c.IsTrue).SetElementName("is_true");
                cm.MapMember(c => c.Text).SetElementName("text");
            });
            BsonClassMap.RegisterClassMap<Domain.GRPC.TestModuleGRPC.Recommendation>(cm =>
            {
                cm.MapMember(c => c.Title).SetElementName("title");
                cm.MapMember(c => c.Description).SetElementName("description");
                cm.MapMember(c => c.Link).SetElementName("link");
            });
        }
    }
}
