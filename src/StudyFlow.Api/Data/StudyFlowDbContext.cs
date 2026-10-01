using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Data;

public class StudyFlowDbContext(DbContextOptions<StudyFlowDbContext> options): DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>()
            .HasOne(task => task.Subject)
            .WithMany(subject => subject.Tasks)
            .HasForeignKey(task => task.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Subject> Subjects { get; set; }
}

