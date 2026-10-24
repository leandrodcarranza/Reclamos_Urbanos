
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public class CategoryManager : ICategoryManager
{
    private readonly ICategoryStore _store;

    public CategoryManager(ICategoryStore store)
    {
        _store = store;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _store.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        return await _store.GetByIdAsync(id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        ValidarNombre(category.Name);

        var categories = await _store.GetAllAsync();

        if (categories.Any(c =>
            string.Equals(c.Name, category.Name.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Ya existe una categoría con ese nombre.");
        }

        category.Name = category.Name.Trim();

        return await _store.AddAsync(category);
    }

    public async Task UpdateAsync(Category category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        if (category.Id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        ValidarNombre(category.Name);

        var existing = await _store.GetByIdAsync(category.Id);

        if (existing == null)
            throw new KeyNotFoundException("La categoría no existe.");

        category.Name = category.Name.Trim();

        var categories = await _store.GetAllAsync();

        if (categories.Any(c =>
            c.Id != category.Id &&
            string.Equals(c.Name, category.Name,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Ya existe otra categoría con ese nombre.");
        }

        await _store.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        var existing = await _store.GetByIdAsync(id);

        if (existing == null)
            throw new KeyNotFoundException("La categoría no existe.");

        await _store.DeleteAsync(id);
    }

    private static void ValidarNombre(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "El nombre de la categoría es obligatorio.");
    }
}
