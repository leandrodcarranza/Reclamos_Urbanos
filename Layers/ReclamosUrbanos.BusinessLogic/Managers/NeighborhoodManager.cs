
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public class NeighborhoodManager : INeighborhoodManager
{
    private readonly INeighborhoodStore _store;

    public NeighborhoodManager(INeighborhoodStore store)
    {
        _store = store;
    }

    public async Task<List<Neighborhood>> GetAllAsync()
    {
        return await _store.GetAllAsync();
    }

    public async Task<Neighborhood?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        return await _store.GetByIdAsync(id);
    }

    public async Task<Neighborhood> AddAsync(Neighborhood neighborhood)
    {
        ArgumentNullException.ThrowIfNull(neighborhood);

        ValidarNombre(neighborhood.Name);

        neighborhood.Name = neighborhood.Name.Trim();

        var neighborhoods = await _store.GetAllAsync();

        if (neighborhoods.Any(n =>
            string.Equals(n.Name, neighborhood.Name,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Ya existe un barrio con ese nombre.");
        }

        return await _store.AddAsync(neighborhood);
    }

    public async Task UpdateAsync(Neighborhood neighborhood)
    {
        ArgumentNullException.ThrowIfNull(neighborhood);

        if (neighborhood.Id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        ValidarNombre(neighborhood.Name);

        neighborhood.Name = neighborhood.Name.Trim();

        var existing = await _store.GetByIdAsync(neighborhood.Id);

        if (existing is null)
            throw new KeyNotFoundException(
                "El barrio no existe.");

        var neighborhoods = await _store.GetAllAsync();

        if (neighborhoods.Any(n =>
            n.Id != neighborhood.Id &&
            string.Equals(n.Name, neighborhood.Name,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Ya existe otro barrio con ese nombre.");
        }

        await _store.UpdateAsync(neighborhood);
    }

    
public async Task DeleteAsync(int id)
{
    if (id <= 0)
        throw new ArgumentException("El ID debe ser mayor a cero.");

    var existing = await _store.GetByIdAsync(id);

    if (existing == null)
        throw new KeyNotFoundException("El barrio no existe.");

    try
    {
        await _store.DeleteAsync(id);
    }
   catch (DbUpdateException ex)
    when (ex.InnerException is PostgresException pg &&
          (pg.SqlState == PostgresErrorCodes.ForeignKeyViolation ||
           pg.SqlState == "23001"))
    {
        throw new InvalidOperationException(
            "No se puede eliminar el barrio porque tiene reclamos asociados.",
            ex);
    }
}

    private static void ValidarNombre(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "El nombre del barrio es obligatorio.");
    }
}
