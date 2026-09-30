using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Mappers;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Services;

public class TaskService : ITaskService
{
    private readonly StudyFlowDbContext _dbContext;

    public TaskService(StudyFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync()
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .OrderBy(task => task.DueDate)
            .ProjectToResponse()
            .ToListAsync();
    }

    public async Task<TaskResponse?> GetByIdAsync(int id)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .ProjectToResponse()
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TaskResponse>> GetBySubjectIdAsync(int subjectId)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.SubjectId == subjectId)
            .ProjectToResponse()
            .ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _dbContext.Tasks.Add(task);

        await _dbContext.SaveChangesAsync();

        return task;
    }

    public async Task<bool> UpdateAsync(int id, TaskItem updatedTask)
    {
        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(task => task.Id == id);

        if (task is null)
        {
            return false;
        }

        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.DueDate = updatedTask.DueDate;
        task.Priority = updatedTask.Priority;
        task.Status = updatedTask.Status;
        task.SubjectId = updatedTask.SubjectId;
        task.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(task => task.Id == id);

        if (task is null)
        {
            return false;
        }

        _dbContext.Tasks.Remove(task);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}
