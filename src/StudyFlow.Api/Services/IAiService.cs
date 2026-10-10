
namespace StudyFlow.Api.Services;

public interface IAiService
{
    Task<string> TestAsync();

    Task<string> GenerateStudyPlanAsync();
}

