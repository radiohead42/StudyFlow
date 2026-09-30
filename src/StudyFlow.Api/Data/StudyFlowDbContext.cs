using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Data;

public class StudyFlowDbContext(DbContextOptions<StudyFlowDbContext> options): DbContext(options)
{
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Subject> Subjects { get; set; }
}

