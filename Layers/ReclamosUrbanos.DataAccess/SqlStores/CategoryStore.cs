
using ReclamosUrbanos.Entities;
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.DataAccess.Stores.Sql;

namespace ReclamosUrbanos.DataAccess.SqlStores;

public class CategoryStore
    : BaseSqlStore<Category>, ICategoryStore
{
    public CategoryStore(AppDbContext context)
        : base(context)
    {
    }
}