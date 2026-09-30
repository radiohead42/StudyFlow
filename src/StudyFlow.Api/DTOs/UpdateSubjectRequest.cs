using System.ComponentModel.DataAnnotations;

namespace StudyFlow.Api.DTOs;

/// <summary>
/// Represents a request to update a subject in the system.
/// </summary>
public class UpdateSubjectRequest
{
    /// <summary>
    /// Gets or sets the name of the subject.
    /// </summary>
    /// <remarks>
    /// The name must be between 2 and 100 characters in length.
    /// </remarks>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the teacher associated with the subject.
    /// </summary>
    /// <remarks>
    /// The teacher name must be up to 100 characters in length.
    /// </remarks>
    [StringLength(100)]
    public string Teacher { get; set; } = string.Empty;
}
