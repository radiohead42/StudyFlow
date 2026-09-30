using System.ComponentModel.DataAnnotations;
using StudyFlow.Api.Models.Enums;

namespace StudyFlow.Api.DTOs;

public class UpdateTaskRequest
{
    [Required]
    [StringLength(100, ErrorMessage = "Title must be at most 100 characters long")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public StudyTaskStatus Status { get; set; }

    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }
}
