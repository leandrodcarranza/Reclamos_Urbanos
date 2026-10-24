
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public interface ICategoryManager
{
    Task<List<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<Category> AddAsync(Category category);

    Task UpdateAsync(Category category);

    Task DeleteAsync(int id);
}
