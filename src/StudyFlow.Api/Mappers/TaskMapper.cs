using Riok.Mapperly.Abstractions;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TaskMapper
{
    public static partial TaskResponse ToResponse(TaskItem task);

    public static partial IQueryable<TaskResponse> ProjectToResponse(
            this IQueryable<TaskItem> query);

    private static partial SubjectSummaryResponse ToSubjectSummary(
            Subject subject);
}
