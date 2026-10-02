using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.DTOs.Queries;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Mappers;
using StudyFlow.Api.Models;
using StudyFlow.Api.Services.CurrentUser;

namespace StudyFlow.Api.Services;

public class TaskService : ITaskService
{
    private readonly StudyFlowDbContext _dbContext;
    private readonly ICurrentUserService currentUserService;

    public TaskService(StudyFlowDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        this.currentUserService = currentUserService;
    }

    public async Task<PagedResponse<TaskResponse>> GetAllAsync(
            TaskQueryParameters parameters)
    {
        var query = _dbContext.Tasks
            .AsNoTracking();

        if (parameters.Status.HasValue)
        {
            query = query.Where(task =>
                    task.Status == parameters.Status.Value);
        }

        if (parameters.Priority.HasValue)
        {
            query = query.Where(task =>
                    task.Priority == parameters.Priority.Value);
        }

        if (parameters.SubjectId.HasValue)
        {
            query = query.Where(task =>
                    task.SubjectId == parameters.SubjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(task =>
                    EF.Functions.ILike(
                        task.Title,
                        $"%{search}%") ||
                    EF.Functions.ILike(
                        task.Description,
                        $"%{search}%"));
        }

        if (parameters.DueFrom.HasValue)
        {
            query = query.Where(task =>
                    task.DueDate >= parameters.DueFrom.Value);
        }

        if (parameters.DueTo.HasValue)
        {
            query = query.Where(task =>
                    task.DueDate <= parameters.DueTo.Value);
        }

        var totalItems = await query.CountAsync();

        query = (parameters.SortBy, parameters.Direction) switch
        {
            (TaskSortBy.Title, SortDirection.Asc) =>
                query
                .OrderBy(task => task.Title)
                .ThenBy(task => task.Id),

            (TaskSortBy.Title, SortDirection.Desc) =>
                query
                .OrderByDescending(task => task.Title)
                .ThenBy(task => task.Id),

            (TaskSortBy.CreatedAt, SortDirection.Asc) =>
                query
                .OrderBy(task => task.CreatedAt)
                .ThenBy(task => task.Id),

            (TaskSortBy.CreatedAt, SortDirection.Desc) =>
                query
                .OrderByDescending(task => task.CreatedAt)
                .ThenBy(task => task.Id),

            (TaskSortBy.Priority, SortDirection.Asc) =>
                query
                .OrderBy(task => task.Priority)
                .ThenBy(task => task.Id),

            (TaskSortBy.Priority, SortDirection.Desc) =>
                query
                .OrderByDescending(task => task.Priority)
                .ThenBy(task => task.Id),

            (TaskSortBy.Status, SortDirection.Asc) =>
                query
                .OrderBy(task => task.Status)
                .ThenBy(task => task.Id),

            (TaskSortBy.Status, SortDirection.Desc) =>
                query
                .OrderByDescending(task => task.Status)
                .ThenBy(task => task.Id),

            (TaskSortBy.DueDate, SortDirection.Desc) =>
                query
                .OrderByDescending(task => task.DueDate)
                .ThenBy(task => task.Id),

            _ =>
                query
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Id)
        };

        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ProjectToResponse()
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(
                totalItems / (double)parameters.PageSize);

        return new PagedResponse<TaskResponse>
        {
            Items = items,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = parameters.Page > 1,
            HasNextPage = parameters.Page < totalPages
        };
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
