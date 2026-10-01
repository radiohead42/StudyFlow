using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Services;

public interface ISubjectService
{
    Task<IReadOnlyList<SubjectResponse>> GetAllAsync(string userId);

    Task<SubjectResponse?> GetByIdAsync(int id, string userId);

    Task<Subject> CreateAsync(Subject subject);

    Task<bool> UpdateAsync(int id, Subject subject);

    Task<bool> DeleteAsync(int id);

    Task<bool> HasTasksAsync(int id);
}
