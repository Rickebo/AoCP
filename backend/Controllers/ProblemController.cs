using Backend.Problems;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProblemController(ProblemService problemService) : ControllerBase
{
    [HttpPost("solve/{year}/{setName}/{problemName}")]
    public async Task<IActionResult> Solve(
        [FromRoute] string year,
        [FromRoute] string setName,
        [FromRoute] string problemName
    )
    {
        var input = await new StreamReader(Request.Body).ReadToEndAsync();
        var id = new ProblemId()
        {
            Year = int.Parse(year),
            SetName = setName,
            ProblemName = problemName
        };

        var problem = problemService.GetProblem(id);

        if (problem == null)
            return NotFound();

        try
        {
            var solution = await Task.Run(() => problem.Solve(input));

            return Ok(
                new ProblemOutput
                {
                    Error = null,
                    Solution = solution,
                    Successful = true
                }
            );
        }
        catch (Exception ex)
        {
            return Ok(
                new ProblemOutput
                {
                    Error = ex.Message,
                    Solution = null,
                    Successful = false
                }
            );
        }
    }
}