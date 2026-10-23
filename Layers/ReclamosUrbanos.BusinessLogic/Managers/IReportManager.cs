
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public interface IReportManager
{
    Task<List<Report>> GetAllAsync();

    Task<List<Report>> GetFilteredAsync(
        int? categoryId,
        int? neighborhoodId,
        Status? status,
        Priority? priority);

    Task<Report?> GetByIdAsync(int id);

    Task<Report> AddAsync(Report report);

    Task UpdateAsync(Report report);

    Task DeleteAsync(int id);

    Task ChangeStatusAsync(int id, Status newStatus);
}
