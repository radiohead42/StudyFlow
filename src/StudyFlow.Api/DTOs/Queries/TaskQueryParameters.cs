using System.ComponentModel.DataAnnotations;
using StudyFlow.Api.Models.Enums;

namespace StudyFlow.Api.DTOs.Queries;

public class TaskQueryParameters
{
    public StudyTaskStatus? Status { get; set; }

    public TaskPriority? Priority { get; set; }

    [Range(1, int.MaxValue)]
    public int? SubjectId { get; set; }

    public string? Search { get; set; }

    public DateTime? DueFrom { get; set; }

    public DateTime? DueTo { get; set; }

    public TaskSortBy SortBy { get; set; } = TaskSortBy.DueDate;

    public SortDirection Direction { get; set; } = SortDirection.Asc;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
