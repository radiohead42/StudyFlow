using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.DTOs;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Mappers;
using StudyFlow.Api.Models;
using StudyFlow.Api.Services;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService taskService, ISubjectService subjectService) : ControllerBase 
{
    private readonly ITaskService taskService = taskService;
    private readonly ISubjectService subjectService = subjectService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll()
    {
        return Ok(await taskService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetById(int id)
    {
        var task = await taskService.GetByIdAsync(id);

        if (task is null) return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(CreateTaskRequest request)
    {
        var subject = await subjectService.GetByIdAsync(request.SubjectId);

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
            IsCompleted = false,
            SubjectId = request.SubjectId
        };

        var createTask = await taskService.CreateAsync(task);

        return CreatedAtAction(
                nameof(GetById),
                new { id = createTask.Id },
                createTask
                );
    }

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
            IsCompleted = request.IsCompleted,
            SubjectId = request.SubjectId
        };

        var update = await taskService.UpdateAsync(id, task);

        if (!update) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var delete = await taskService.DeleteAsync(id);

        if (!delete) return NotFound();

        return NoContent();
    }
}
