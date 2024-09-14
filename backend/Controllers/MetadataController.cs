using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class MetadataController : ControllerBase
{
    private readonly ProblemService _problemService;

    public MetadataController(ProblemService problemService)
    {
        _problemService = problemService;
    }
    
    [Route("")]
    public async Task<IActionResult> GetYears()
    {
        return Ok(_problemService.GetMetadata());
    }
}