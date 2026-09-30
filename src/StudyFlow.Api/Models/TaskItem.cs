using StudyFlow.Api.Models.Enums;

namespace StudyFlow.Api.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset DueDate { get; set; }
    public TaskPriority Priority { get; set; }
    public StudyTaskStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
}
