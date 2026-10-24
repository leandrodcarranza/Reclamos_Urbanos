
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess.Stores;

public interface INeighborhoodStore
{
    Task<List<Neighborhood>> GetAllAsync();

    Task<Neighborhood?> GetByIdAsync(int id);

    Task<Neighborhood> AddAsync(Neighborhood entity);

    Task UpdateAsync(Neighborhood entity);

    Task DeleteAsync(int id);
}
