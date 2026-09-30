using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetAllAsync();

    Task<TaskResponse?> GetByIdAsync(int id);

    Task<IReadOnlyList<TaskResponse>> GetBySubjectIdAsync(int id);

    Task<TaskItem> CreateAsync(TaskItem task);

    Task<bool> UpdateAsync(int id, TaskItem task);

    Task<bool> DeleteAsync(int id);
}
