using Microsoft.EntityFrameworkCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Mappers;
using StudyFlow.Api.Models;
using StudyFlow.Api.DTOs.Responses;

namespace StudyFlow.Api.Services;

public class SubjectService(StudyFlowDbContext studyFlowDbContext) : ISubjectService
{
    public async Task<Subject> CreateAsync(Subject subject)
    {
        studyFlowDbContext.Subjects.Add(subject);
        await studyFlowDbContext.SaveChangesAsync();
        return subject;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subject = await studyFlowDbContext.Subjects
            .FirstOrDefaultAsync(subject => subject.Id == id);
        
        if (subject is null) return false;

        studyFlowDbContext.Remove(subject);
        await studyFlowDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<SubjectResponse>> GetAllAsync()
    {
        return await studyFlowDbContext.Subjects
            .AsNoTracking()
            .OrderBy(subject => subject.Name)
            .ProjectToResponse()
            .ToListAsync();
    }

    public async Task<SubjectResponse?> GetByIdAsync(int id)
    {
        return await studyFlowDbContext.Subjects
            .AsNoTracking()
            .ProjectToResponse()
            .FirstOrDefaultAsync(subject => subject.Id == id);
    }

    public async Task<bool> UpdateAsync(int id, Subject updateSubject)
    {
        var subject = await studyFlowDbContext.Subjects
            .FirstOrDefaultAsync(subject => subject.Id == id);

        if (subject is null) return false;

        subject.Name = updateSubject.Name;
        subject.Teacher = updateSubject.Teacher;

        await studyFlowDbContext.SaveChangesAsync();
        return true;
    }
}
