
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess.Stores;

public interface ICategoryStore
{
    Task<List<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<Category> AddAsync(Category entity);

    Task UpdateAsync(Category entity);

    Task DeleteAsync(int id);
}
