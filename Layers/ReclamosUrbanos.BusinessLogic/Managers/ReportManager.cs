
using ReclamosUrbanos.Entities;
using ReclamosUrbanos.DataAccess.Stores;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public class ReportManager : IReportManager
{
    private readonly IReportStore _reportStore;

    public ReportManager(IReportStore reportStore)
    {
        _reportStore = reportStore;
    }

    public async Task<List<Report>> GetAllAsync()
    {
        return await _reportStore.GetAllAsync();
    }

    public async Task<List<Report>> GetFilteredAsync(
        int? categoryId,
        int? neighborhoodId,
        Status? status,
        Priority? priority)
    {
        if (categoryId.HasValue && categoryId.Value <= 0)
            throw new ArgumentException("La categoría debe ser válida.");

        if (neighborhoodId.HasValue && neighborhoodId.Value <= 0)
            throw new ArgumentException("El barrio debe ser válido.");

        if (status.HasValue &&
            !Enum.IsDefined(typeof(Status), status.Value))
            throw new ArgumentException("El estado no es válido.");

        if (priority.HasValue &&
            !Enum.IsDefined(typeof(Priority), priority.Value))
            throw new ArgumentException("La prioridad no es válida.");

        var reports = await _reportStore.GetAllAsync();

        if (categoryId.HasValue)
            reports = reports
                .Where(r => r.CategoryId == categoryId.Value)
                .ToList();

        if (neighborhoodId.HasValue)
            reports = reports
                .Where(r => r.NeighborhoodId == neighborhoodId.Value)
                .ToList();

        if (status.HasValue)
            reports = reports
                .Where(r => r.Status == status.Value)
                .ToList();

        if (priority.HasValue)
            reports = reports
                .Where(r => r.Priority == priority.Value)
                .ToList();

        return reports;
    }

    public async Task<Report?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        return await _reportStore.GetByIdAsync(id);
    }

    public async Task<Report> AddAsync(Report report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        if (string.IsNullOrWhiteSpace(report.Description))
            throw new ArgumentException("La descripción es obligatoria.");
        ValidateAddress(report);   // NUEVO

        if (report.CategoryId <= 0)
            throw new ArgumentException("Debe indicar una categoría válida.");

        if (report.NeighborhoodId <= 0)
            throw new ArgumentException("Debe indicar un barrio válido.");
        

        return await _reportStore.AddAsync(report);
    }

    public async Task UpdateAsync(Report report)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        if (report.Id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(report.Description))
            throw new ArgumentException("La descripción es obligatoria.");
        ValidateAddress(report);

        if (report.CategoryId <= 0 ||
            report.NeighborhoodId <= 0)
            throw new ArgumentException(
                "La categoría y el barrio deben ser válidos.");

        var existingReport =
            await _reportStore.GetByIdAsync(report.Id);

        if (existingReport == null)
            throw new KeyNotFoundException("El reclamo no existe.");

        await _reportStore.UpdateAsync(report);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        var report = await _reportStore.GetByIdAsync(id);

        if (report == null)
            throw new KeyNotFoundException("El reclamo no existe.");

        await _reportStore.DeleteAsync(id);
    }

    public async Task ChangeStatusAsync(int id, Status newStatus)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor a cero.");

        if (!Enum.IsDefined(typeof(Status), newStatus))
            throw new ArgumentException("El estado indicado no es válido.");

        var report = await _reportStore.GetByIdAsync(id);

        if (report == null)
            throw new KeyNotFoundException("El reclamo no existe.");

        report.Status = newStatus;

        await _reportStore.UpdateAsync(report);
    }

    private static void ValidateAddress(Report report)
    {
        if (string.IsNullOrWhiteSpace(report.Address))
            throw new ArgumentException("La dirección es obligatoria.");

        report.Address = report.Address.Trim();

        if (report.Address.Length > 200)
            throw new ArgumentException(
                "La dirección no puede superar los 200 caracteres.");
    }
}
