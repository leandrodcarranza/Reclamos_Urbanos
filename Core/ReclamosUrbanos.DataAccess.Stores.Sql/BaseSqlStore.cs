using Microsoft.EntityFrameworkCore;

namespace ReclamosUrbanos.DataAccess.Stores.Sql;

public abstract class BaseSqlStore<TEntity> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> Set;

    protected BaseSqlStore(DbContext context)
    {
        Context = context;
        Set = context.Set<TEntity>();
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await Set.AsNoTracking().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await Set.AsNoTracking()
            .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        Set.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        Set.Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await Set.FindAsync(id);
        if (entity is null) return;

        Set.Remove(entity);
        await Context.SaveChangesAsync();
    }
}
