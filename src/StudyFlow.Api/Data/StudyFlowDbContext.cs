using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Models;
using StudyFlow.Api.Models.Identity;

namespace StudyFlow.Api.Data;

public class StudyFlowDbContext(DbContextOptions<StudyFlowDbContext> options): IdentityDbContext<ApplicationUser>(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>()
            .HasOne(task => task.Subject)
            .WithMany(subject => subject.Tasks)
            .HasForeignKey(task => task.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subject>()
            .HasOne(subject => subject.User)
            .WithMany(user => user.Subjects)
            .HasForeignKey(subject => subject.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Subject> Subjects { get; set; }
}
