using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.BusinessLogic.Services;

public interface IAiService
{
    /// <summary>
    /// Analiza la descripción de un reclamo y sugiere categoría, prioridad y resumen.
    /// La categoría sugerida debe ser una de las recibidas.
    /// </summary>
    Task<ReportAnalysis> AnalyzeReportAsync(
        string description,
        IReadOnlyList<Category> categories,
        CancellationToken cancellationToken = default);
}
