
using ReclamosUrbanos.DataAccess.Stores.Sql;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess.Stores;

public class ReportStore : BaseSqlStore<Report>, IReportStore
{
    public ReportStore(AppDbContext context) : base(context)
    {
    }
}
