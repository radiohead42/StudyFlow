using Riok.Mapperly.Abstractions;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Models;

namespace StudyFlow.Api.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class SubjectMapper
{
    public static partial SubjectResponse ToResponse(
        Subject subject);

    public static partial IQueryable<SubjectResponse> ProjectToResponse(
        this IQueryable<Subject> query);
}
