using TT = TestApi.Domain.TestTemplateDocument;
using TTS = TestApi.Domain.TestTemplateStatisticsDocument;
using TR = TestApi.Domain.TestResultDocument;
using TestApi.Domain.TestResultDocument;

namespace TestApi.Services
{
    public interface ITemplateStatisticsService
    {
        TTS.TemplateStatistics CreateTemplateStatistics(TT.TestTemplate testTemplate, TR.TestResult testResult);
    }

    public class TemplateStatisticsService : ITemplateStatisticsService
    {

        public TTS.TemplateStatistics CreateTemplateStatistics(TT.TestTemplate testTemplate, TR.TestResult testResult)
        {
            TTS.TemplateStatistics statistics = new TTS.TemplateStatistics();

            statistics.Title = testTemplate.Title;
            statistics.Competencies = new List<TTS.Competency>();
            statistics.IsPassed = true;

            int count_passed_competence = 0;

            foreach (var c in testTemplate.Competencies)
            {
                bool is_passed_competence = true;
                int count_passed_slill = 0;

                TTS.Competency comp = new TTS.Competency();
                comp.Title = c.Title;
                comp.RequiredCode = c.RequiredCode;
                comp.Skills = new List<TTS.Skill>();

                foreach(var s in c.Skills)
                {
                    TTS.Skill skill = new TTS.Skill();
                    skill.Title = s.Title;
                    skill.RequiredCode = s.RequiredCode;
                    skill.IdTestModule = s.IdTestModule;

                    TR.TestModuleResult testModuleResult =testResult.TestModuleResult.FirstOrDefault(r => r.TestModuleId == s.IdTestModule);

                    skill.IsPassed = testModuleResult.UserResult.IsPassed;

                    if(testModuleResult.UserResult.Values != 0)
                        skill.Percent = (double)((double)testModuleResult.UserResult.Values / (double)testModuleResult.MaxValue) * 100.0;

                    if (skill.RequiredCode == (int)SKILL_NEED.REQUIRE_HARD && !skill.IsPassed)
                        is_passed_competence = false;

                    if(skill.IsPassed) count_passed_slill++;

                    comp.Skills.Add(skill);
                }
                comp.IsPassed = is_passed_competence;

                if(count_passed_slill > 0)
                    comp.Percent = (double)((double)count_passed_slill / (double)comp.Skills.Count) * 100.0;

                if(comp.RequiredCode == (int)COMPETENCE_NEED.REQUIRE_HARD && !comp.IsPassed)
                    statistics.IsPassed = false;

                if(comp.IsPassed) count_passed_competence++;

                statistics.Competencies.Add(comp);
            }

            if (count_passed_competence > 0)
                statistics.Percent = (double)((double)count_passed_competence / (double)statistics.Competencies.Count) * 100.0;

            return statistics;
        }
    }


    public enum SKILL_NEED
    {
        REQUIRE_HARD = 1,
        REQUIRE_MIDDLE = 2,
        REQUIRE_SOFT = 3
    }

    public enum COMPETENCE_NEED
    {
        REQUIRE_HARD = 1,
        REQUIRE_MIDDLE = 2,
        REQUIRE_SOFT = 3
    }
}
