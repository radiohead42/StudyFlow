using StudyFlow.Api.DTOs.Responses;

namespace StudyFlow.Api.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync();
}
