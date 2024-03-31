using StatisticApi.Repository.DocumentDB;

namespace Statistic.Test.Repository
{
    public class UnitTestStatisticRepository
    {
        //[Fact]
        //public async void InsertStatistic()
        //{
        //    var repository = new StatisticRepository("mongodb://localhost:27017", "Statistic");

        //    string? resultId = await repository.InsertStatistic(new Domain.UserStatistic
        //    {
        //        User = new Domain.User
        //        {
        //            UserId = 1,
        //            Username = "Test",
        //            Email = "Test"
        //        },
        //        Competencies = new List<Domain.Competency> { new Domain.Competency
        //            {
        //                Title = "TestCompetency",
        //                DateCreate = DateTime.Now,
        //                Percent = 56
        //            }
        //        },
        //        Skills = new List<Domain.Skill> { new Domain.Skill
        //            {
        //                Title = "TestSkill",
        //                DateCreate = DateTime.Now,
        //                Percent = 56
        //            }
        //        }
        //    });

        //    Assert.NotNull(resultId);
        //}


        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public async void GetStatistic(int id)
        {
            var repository = new StatisticRepository("mongodb://localhost:27017", "Statistic");

            var result = await repository.GetStatistic(id);

            if(id > 0) Assert.NotNull(result);
            else Assert.Null(result);
        }


        //[Fact]
        //public async void UpdateStatistic()
        //{
        //    var repository = new StatisticRepository("mongodb://localhost:27017", "Statistic");

        //    bool result = await repository.UpdateStatisticCompetenceAndSkill(new Domain.UserStatistic
        //    {
        //        User = new Domain.User
        //        {
        //            UserId = 1
        //        },
        //        Competencies = new List<Domain.Competency> { new Domain.Competency
        //            {
        //                Title = "NewTestCompetency",
        //                DateCreate = DateTime.Now,
        //                Percent = 52
        //            }
        //        },
        //        Skills = new List<Domain.Skill> { new Domain.Skill
        //            {
        //                Title = "NewTestSkill",
        //                DateCreate = DateTime.Now,
        //                Percent = 57
        //            },
        //            new Domain.Skill 
        //            {
        //                Title = "NewTestSkill2",
        //                DateCreate = DateTime.Now,
        //                Percent = 34
        //            }
        //        },
        //    });

        //    Assert.True(result);
        //}

        //[Theory]
        //[InlineData("65f098f92c68d2952c9642a5")]
        //public async void DeleteStatistic(string id)
        //{
        //    var repository = new StatisticRepository("mongodb://localhost:27017", "Statistic");

        //    var result = await repository.DeleteStatistic(id);

        //    Assert.True(result);
        //}

    }
}