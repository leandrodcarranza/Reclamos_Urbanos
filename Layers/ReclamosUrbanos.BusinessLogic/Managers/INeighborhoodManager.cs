
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public interface INeighborhoodManager
{
    Task<List<Neighborhood>> GetAllAsync();

    Task<Neighborhood?> GetByIdAsync(int id);

    Task<Neighborhood> AddAsync(Neighborhood neighborhood);

    Task UpdateAsync(Neighborhood neighborhood);

    Task DeleteAsync(int id);
}
