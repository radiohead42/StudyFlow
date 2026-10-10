using Microsoft.AspNetCore.Mvc;
using StudyFlow.Api.Data;
using StudyFlow.Api.Services;

namespace StudyFlow.Api.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController: ControllerBase
{
    private readonly IAiService aiService;

    public AiController(IAiService aiService)
    {
        this.aiService = aiService;
    }

    [HttpGet("test")]
    public async Task<ActionResult> Test()
    {
        var result = await aiService.TestAsync();

        return Ok(result);
    }

    [HttpGet("study-plan")]
    public async Task<IActionResult>
        StudyPlan()
        {
            var result =
                await aiService
                .GenerateStudyPlanAsync();

            return Ok(result);
        }
}
