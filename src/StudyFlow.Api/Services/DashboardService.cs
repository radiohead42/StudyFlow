using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Models.Enums;

namespace StudyFlow.Api.Services;

public class DashboardService(
    StudyFlowDbContext dbContext) : IDashboardService
{
    public async Task<DashboardResponse> GetAsync()
    {
        var now = DateTime.UtcNow;
        var sevenDaysFromNow = now.AddDays(7);

        var totalTasks =
            await dbContext.Tasks.CountAsync();

        var pending =
            await dbContext.Tasks.CountAsync(task =>
                task.Status == StudyTaskStatus.Pending);

        var inProgress =
            await dbContext.Tasks.CountAsync(task =>
                task.Status == StudyTaskStatus.InProgress);

        var completed =
            await dbContext.Tasks.CountAsync(task =>
                task.Status == StudyTaskStatus.Completed);

        var cancelled =
            await dbContext.Tasks.CountAsync(task =>
                task.Status == StudyTaskStatus.Cancelled);

        var overdue =
            await dbContext.Tasks.CountAsync(task =>
                task.DueDate < now &&
                task.Status != StudyTaskStatus.Completed &&
                task.Status != StudyTaskStatus.Cancelled);

        var dueNext7Days =
            await dbContext.Tasks.CountAsync(task =>
                task.DueDate >= now &&
                task.DueDate <= sevenDaysFromNow &&
                task.Status != StudyTaskStatus.Completed &&
                task.Status != StudyTaskStatus.Cancelled);

        return new DashboardResponse
        {
            TotalTasks = totalTasks,
            Pending = pending,
            InProgress = inProgress,
            Completed = completed,
            Cancelled = cancelled,
            Overdue = overdue,
            DueNext7Days = dueNext7Days
        };
    }
}
