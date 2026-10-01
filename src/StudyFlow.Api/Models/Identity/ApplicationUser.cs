using Microsoft.AspNetCore.Identity;

namespace StudyFlow.Api.Models.Identity;

public class ApplicationUser: IdentityUser
{
    public ICollection<Subject> Subjects { get; set; } = [];
}
