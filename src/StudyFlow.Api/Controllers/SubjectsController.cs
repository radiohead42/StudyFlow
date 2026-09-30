using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Models;
using StudyFlow.Api.Services;

namespace StudyFlow.Api.Controllers;

/// <summary>
/// Initializes a new instance of the <see cref="SubjectController"/> class.
/// </summary>
/// <param name="subjectService">The subject service.</param>
/// <param name="taskService">The task service.</param>
[ApiController]
[Route("api/subjects")]

public class SubjectController(ISubjectService subjectService, ITaskService taskService) : ControllerBase
{
    private readonly ISubjectService subjectService = subjectService;
    private readonly ITaskService taskService = taskService;

    /// <summary>
    /// Gets all subjects.
    /// </summary>
    /// <returns>A list of subject responses.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetAll()
    {
        var subjects =
            await subjectService.GetAllAsync();

        return Ok(subjects);
    }

    /// <summary>
        /// Gets a subject by ID.
        /// </summary>
        /// <param name="id">The ID of the subject.</param>
        /// <returns>Details of the subject.</returns>
        /// <example>
        /// <response>
        ///     {
        ///         "Id": 1,
        ///         "Name": "Mathematics",
        ///         "Teacher": "Dr. Smith"
        ///     }
        /// </response>
        /// </example>
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

    /// <summary>
    /// Gets tasks for a subject by its ID.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <returns>A list of task responses.</returns>
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

    /// <summary>
        /// Creates a new subject.
        /// </summary>
        /// <param name="request">The request object containing subject details.</param>
        /// <returns>Created subject details.</returns>
        /// <example>
        /// <request>
        ///     {
        ///         "Name": "Mathematics",
        ///         "Teacher": "Dr. Smith"
        ///     }
        /// </request>
        /// <response>
        ///     {
        ///         "Id": 1,
        ///         "Name": "Mathematics",
        ///         "Teacher": "Dr. Smith"
        ///     }
        /// </response>
        /// </example>
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

    /// <summary>
    /// Updates an existing subject.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <param name="request">The update subject request.</param>
    /// <returns>No content.</returns>
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

    /// <summary>
    /// Deletes a subject.
    /// </summary>
    /// <param name="id">The ID of the subject.</param>
    /// <returns>No content.</returns>
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
