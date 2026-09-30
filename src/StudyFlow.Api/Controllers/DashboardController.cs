using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.DTOs.Responses;
using StudyFlow.Api.Services;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(
    IDashboardService dashboardService) : ControllerBase
{
    /// <summary>
    /// Obtiene un resumen general de las tareas.
    /// </summary>
    /// <remarks>
    /// Incluye el número total de tareas, tareas pendientes,
    /// en progreso, completadas, canceladas, vencidas y
    /// próximas a vencer durante los siguientes siete días.
    /// </remarks>
    /// <response code="200">
    /// El resumen fue generado correctamente.
    /// </response>
    [HttpGet]
    [ProducesResponseType<DashboardResponse>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        var dashboard =
            await dashboardService.GetAsync();

        return Ok(dashboard);
    }
}
