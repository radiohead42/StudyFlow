using System.ComponentModel.DataAnnotations;

namespace StudyFlow.Api.DTOs;

public class UpdateSubjectRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string Teacher { get; set; } = string.Empty;
}
