
using Microsoft.AspNetCore.Mvc;
using ReclamosUrbanos.BusinessLogic.Managers;
using ReclamosUrbanos.Entities;
using ReclamosUrbanos.Presentation.DTOs;

namespace ReclamosUrbanos.Presentation.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    private readonly IReportManager _reportManager;

    public ReportController(IReportManager reportManager)
    {
        _reportManager = reportManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] int? neighborhoodId,
        [FromQuery] Status? status,
        [FromQuery] Priority? priority)
    {
        try
        {
            var reports = await _reportManager.GetFilteredAsync(
                categoryId,
                neighborhoodId,
                status,
                priority);

            return Ok(reports.Select(ToOutput).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var report = await _reportManager.GetByIdAsync(id);

            if (report == null)
                return NotFound("El reclamo no existe.");

            return Ok(ToOutput(report));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateReportInput input)
    {
        try
        {
            var report = new Report
            {
                Description = input.Description,
                CategoryId = input.CategoryId,
                NeighborhoodId = input.NeighborhoodId
            };

            var created = await _reportManager.AddAsync(report);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ToOutput(created));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateReportInput input)
    {
        try
        {
            var report = await _reportManager.GetByIdAsync(id);

            if (report == null)
                return NotFound("El reclamo no existe.");

            report.Description = input.Description;
            report.CategoryId = input.CategoryId;
            report.NeighborhoodId = input.NeighborhoodId;

            await _reportManager.UpdateAsync(report);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _reportManager.DeleteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(
        int id,
        [FromBody] Status newStatus)
    {
        try
        {
            await _reportManager.ChangeStatusAsync(id, newStatus);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    private static ReportOutput ToOutput(Report report)
    {
        return new ReportOutput
        {
            Id = report.Id,
            Description = report.Description,
            Summary = report.Summary,
            CategoryId = report.CategoryId,
            NeighborhoodId = report.NeighborhoodId,
            Status = report.Status,
            Priority = report.Priority,
            CreatedAt = report.CreatedAt
        };
    }
}
