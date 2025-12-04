using System.Threading.Tasks;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class MetadataController(ProblemService problemService) : ControllerBase
{
    private readonly ProblemService _problemService = problemService;

    [Route("")]
    public async Task<IActionResult> GetYears()
    {
        var meta = _problemService.GetMetadata();
        _problemService.Validate(meta);
        return Ok(meta);
    }
}

