using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.DTOs.Queries;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Models;
using StudyFlow.Api.Models.Enums;
using StudyFlow.Api.Services;

namespace StudyFlow.Api.Controllers;

/// <summary>
/// Controller for managing tasks.
/// </summary>
[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService taskService, ISubjectService subjectService) : ControllerBase 
{
    private readonly ITaskService taskService = taskService;
    private readonly ISubjectService subjectService = subjectService;

    /// <summary>
    /// Obtiene las tareas aplicando filtros, ordenamiento y paginación.
    /// </summary>
    /// <remarks>
    /// Permite filtrar las tareas por estado, prioridad,
    /// materia, texto y rango de fechas.
    /// </remarks>
    /// <response code="200">
    /// Las tareas se obtuvieron correctamente.
    /// </response>
    [HttpGet]
    [ProducesResponseType<PagedResponse<TaskResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TaskResponse>>> GetAll(
            [FromQuery] TaskQueryParameters parameters)
    {
        var result =
            await taskService.GetAllAsync(parameters);

        return Ok(result);
    }
    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    /// <param name="id">The ID of the task.</param>
    /// <returns>The task with the specified ID.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetById(int id)
    {
        var task = await taskService.GetByIdAsync(id);

        if (task is null) return NotFound();

        return Ok(task);
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request">The request containing the details of the new task.</param>
    /// <returns>The created task.</returns>
    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(CreateTaskRequest request)
    {
        var subject = await subjectService.GetByIdAsync(request.SubjectId);

        var now = DateTimeOffset.UtcNow;

        if (subject is null) return BadRequest("La materia no existe");

        if (request.DueDate < DateTime.Now)
        {

            return BadRequest("La fecha de vencimiento no puede ser anterior a la fecha actual.");
        }

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate.ToUniversalTime(),
            Priority = request.Priority,
            Status = StudyTaskStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now,
            SubjectId = request.SubjectId
        };

        var createTask = await taskService.CreateAsync(task);

        return CreatedAtAction(
                nameof(GetById),
                new { id = createTask.Id },
                createTask
                );
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The ID of the task to update.</param>
    /// <param name="request">The request containing the details of the update.</param>
    /// <returns>No content if the update is successful, otherwise a not found result.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskRequest request)
    {
        var subject = await subjectService.GetByIdAsync(request.SubjectId);

        if (subject is null) return BadRequest("La materia no existe");

        if (request.DueDate < DateTime.Now) return  BadRequest("La fecha de vencimiento no puede ser anterior a la fecha actual.");

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            SubjectId = request.SubjectId
        };

        var update = await taskService.UpdateAsync(id, task);

        if (!update) return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">The ID of the task to delete.</param>
    /// <returns>No content if the deletion is successful, otherwise a not found result.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var delete = await taskService.DeleteAsync(id);

        if (!delete) return NotFound();

        return NoContent();
    }
}
