using ReclamosUrbanos.BusinessLogic.Services;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public interface IReportAnalysisManager
{
    Task<ReportAnalysis> AnalyzeAsync(
        string description,
        CancellationToken cancellationToken = default);
}
