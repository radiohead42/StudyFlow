using System.ComponentModel.DataAnnotations;

namespace StudyFlow.Api.DTOs;

public class CreateTaskRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public DateTimeOffset DueDate { get; set; }
    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }
}
