using System.ComponentModel.DataAnnotations;

namespace StudyFlow.Api.DTOs;

/// <summary>
/// Represents a request for creating a subject.
/// </summary>
public class CreateSubjectRequest
{
    /// <summary>
    /// Gets or sets the name of the subject.
    /// </summary>
    /// <value>The name of the subject, must be between 1 and 100 characters.</value>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the teacher's name.
    /// </summary>
    /// <value>The teacher's name, must be between 1 and 100 characters.</value>
    [StringLength(100)]
    public string Teacher { get; set; } = string.Empty;
}
