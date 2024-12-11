using System.Threading.Tasks;
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
        var meta = _problemService.GetMetadata();
        _problemService.Validate(meta);
        return Ok(meta);
    }
}