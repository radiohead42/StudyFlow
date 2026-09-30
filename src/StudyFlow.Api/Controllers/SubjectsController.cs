using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Mappers;
using StudyFlow.Api.Models;
using StudyFlow.Api.Services;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Route("api/subjects")]
public class SubjectController(ISubjectService subjectService, ITaskService taskService) : ControllerBase
{
    private readonly ISubjectService subjectService = subjectService;
    private readonly ITaskService taskService = taskService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetAll()
    {
        var subjects =
            await subjectService.GetAllAsync();

        return Ok(subjects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetById(int id)
    {
        var subject =
            await subjectService.GetByIdAsync(id);

        if (subject is null)
        {
            return NotFound();
        }

        return Ok(subject);
    }

    [HttpGet("{id:int}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks(int id)
    {
        var subject = await subjectService.GetByIdAsync(id);

        if (subject is null)
        {
            return NotFound();
        }

        var tasks = await taskService.GetBySubjectIdAsync(id);

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<Subject>> Create(CreateSubjectRequest request)
    {
        var subject = new Subject
        {
            Name = request.Name,
            Teacher = request.Teacher
        };

        var createdSubject = await subjectService.CreateAsync(subject);

        return CreatedAtAction(
                nameof(GetById),
                new { id = createdSubject.Id },
                createdSubject
                );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSubjectRequest request)
    {
        var subject = new Subject
        {
            Name = request.Name,
            Teacher = request.Teacher
        };

        var updated = await subjectService.UpdateAsync(id, subject);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await subjectService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

}
