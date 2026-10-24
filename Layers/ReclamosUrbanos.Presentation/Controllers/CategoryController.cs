
using Microsoft.AspNetCore.Mvc;
using ReclamosUrbanos.BusinessLogic.Managers;
using ReclamosUrbanos.Presentation.DTOs;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.Presentation.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryManager _manager;

    public CategoryController(ICategoryManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryOutput>>> GetAll()
    {
        var categories = await _manager.GetAllAsync();

        var result = categories.Select(c => new CategoryOutput
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryOutput>> GetById(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a cero.");

        var category = await _manager.GetByIdAsync(id);

        if (category is null)
            return NotFound("La categoría no existe.");

        return Ok(new CategoryOutput
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }

[HttpPost]
public async Task<ActionResult<CategoryOutput>> Create(
    [FromBody] CreateCategoryInput input)
{
    try
    {
        var category = new Category
        {
            Name = input.Name,
            Description = input.Description
        };

        var created = await _manager.AddAsync(category);

        var output = new CategoryOutput
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description
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
    [FromBody] UpdateCategoryInput input)
{
    if (id <= 0)
        return BadRequest("El ID debe ser mayor a cero.");

    try
    {
        var category = new Category
        {
            Id = id,
            Name = input.Name,
            Description = input.Description
        };

        await _manager.UpdateAsync(category);

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