
using Microsoft.AspNetCore.Mvc;
using ReclamosUrbanos.BusinessLogic.Managers;
using ReclamosUrbanos.Entities;
using ReclamosUrbanos.Presentation.DTOs;

namespace ReclamosUrbanos.Presentation.Controllers;

[ApiController]
[Route("api/neighborhoods")]
public class NeighborhoodController : ControllerBase
{
    private readonly INeighborhoodManager _manager;

    public NeighborhoodController(INeighborhoodManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<ActionResult<List<NeighborhoodOutput>>> GetAll()
    {
        var neighborhoods = await _manager.GetAllAsync();

        var result = neighborhoods.Select(n => new NeighborhoodOutput
        {
            Id = n.Id,
            Name = n.Name
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NeighborhoodOutput>> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a cero.");

        var neighborhood = await _manager.GetByIdAsync(id);

        if (neighborhood is null)
            return NotFound("El barrio no existe.");

        return Ok(new NeighborhoodOutput
        {
            Id = neighborhood.Id,
            Name = neighborhood.Name
        });
    }

    [HttpPost]
    public async Task<ActionResult<NeighborhoodOutput>> Create(
        [FromBody] CreateNeighborhoodInput input)
    {
        try
        {
            var neighborhood = new Neighborhood
            {
                Name = input.Name
            };

            var created = await _manager.AddAsync(neighborhood);

            var output = new NeighborhoodOutput
            {
                Id = created.Id,
                Name = created.Name
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                output);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateNeighborhoodInput input)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a cero.");

        try
        {
            var neighborhood = new Neighborhood
            {
                Id = id,
                Name = input.Name
            };

            await _manager.UpdateAsync(neighborhood);

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
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

   

[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    if (id <= 0)
        return BadRequest("El ID debe ser mayor a cero.");

    try
    {
        await _manager.DeleteAsync(id);

        return NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(ex.Message);
    }
}

}