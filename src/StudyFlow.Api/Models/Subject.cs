using StudyFlow.Api.Models.Identity;

namespace StudyFlow.Api.Models;

/// <summary>
/// Represents a subject in the study flow.
/// </summary>
public class Subject
{
    /// <summary>
    /// Gets or sets the unique identifier of the subject.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the subject.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the teacher associated with the subject.
    /// </summary>
    public string Teacher { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the collection of tasks associated with the subject.
    /// </summary>
    public ICollection<TaskItem> Tasks { get; set; } = [];

    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }
}
