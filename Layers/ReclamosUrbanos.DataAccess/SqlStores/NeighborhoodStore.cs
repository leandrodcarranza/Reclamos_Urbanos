
using ReclamosUrbanos.Entities;
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.DataAccess.Stores.Sql;

namespace ReclamosUrbanos.DataAccess.SqlStores;

public class NeighborhoodStore
    : BaseSqlStore<Neighborhood>, INeighborhoodStore
{
    public NeighborhoodStore(AppDbContext context)
        : base(context)
    {
    }
}
