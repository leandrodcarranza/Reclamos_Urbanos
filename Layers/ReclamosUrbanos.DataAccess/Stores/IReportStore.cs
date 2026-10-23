
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.DataAccess.Stores;

public interface IReportStore
{
    Task<List<Report>> GetAllAsync();

    Task<Report?> GetByIdAsync(int id);

    Task<Report> AddAsync(Report report);

    Task UpdateAsync(Report report);

    Task DeleteAsync(int id);
}
