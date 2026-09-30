namespace StudyFlow.Api.DTOs.Responses;

public class TaskResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public SubjectSummaryResponse Subject { get; set; } = null!;
}
