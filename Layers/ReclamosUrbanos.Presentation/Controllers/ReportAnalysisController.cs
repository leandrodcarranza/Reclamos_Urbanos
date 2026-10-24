using Microsoft.AspNetCore.Mvc;
using ReclamosUrbanos.BusinessLogic.Managers;
using ReclamosUrbanos.Presentation.DTOs;

namespace ReclamosUrbanos.Presentation.Controllers;

[ApiController]
[Route("api/reports/analyze")]
public class ReportAnalysisController : ControllerBase
{
    private readonly IReportAnalysisManager _analysisManager;

    public ReportAnalysisController(IReportAnalysisManager analysisManager)
    {
        _analysisManager = analysisManager;
    }

    [HttpPost]
    public async Task<IActionResult> Analyze(
        [FromBody] AnalyzeReportInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            var analysis = await _analysisManager.AnalyzeAsync(
                input.Description, cancellationToken);

            return Ok(new AnalyzeReportOutput
            {
                CategoryId = analysis.CategoryId,
                Priority = analysis.Priority,
                Summary = analysis.Summary,
                UsedFallback = analysis.UsedFallback
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
