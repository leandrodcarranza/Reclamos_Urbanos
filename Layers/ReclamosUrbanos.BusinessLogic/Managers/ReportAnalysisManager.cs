using ReclamosUrbanos.BusinessLogic.Services;
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.BusinessLogic.Managers;

public class ReportAnalysisManager : IReportAnalysisManager
{
    private const int MaxDescriptionLength = 2000;
    private const int MaxSummaryLength = 200;

    private readonly IAiService _aiService;
    private readonly ICategoryStore _categoryStore;

    public ReportAnalysisManager(IAiService aiService, ICategoryStore categoryStore)
    {
        _aiService = aiService;
        _categoryStore = categoryStore;
    }

    public async Task<ReportAnalysis> AnalyzeAsync(
        string description,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción es obligatoria.");

        description = description.Trim();

        if (description.Length > MaxDescriptionLength)
            throw new ArgumentException(
                $"La descripción no puede superar los {MaxDescriptionLength} caracteres.");

        var categories = await _categoryStore.GetAllAsync();

        try
        {
            var analysis = await _aiService.AnalyzeReportAsync(
                description, categories, cancellationToken);

            return Normalize(analysis, description, categories);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            // Plan B: si la IA falla, la persona completa los datos a mano.
            return Fallback(description);
        }
    }

    private static ReportAnalysis Normalize(
        ReportAnalysis analysis, string description, IReadOnlyList<Category> categories)
    {
        // La categoría sugerida tiene que existir; si no, queda sin definir.
        int? categoryId = analysis.CategoryId.HasValue &&
                          categories.Any(c => c.Id == analysis.CategoryId.Value)
            ? analysis.CategoryId
            : null;

        var priority = Enum.IsDefined(typeof(Priority), analysis.Priority)
            ? analysis.Priority
            : Priority.Medium;

        var summary = (analysis.Summary ?? string.Empty).Trim();
        if (summary.Length == 0)
            summary = description;

        return new ReportAnalysis
        {
            CategoryId = categoryId,
            Priority = priority,
            Summary = Truncate(summary, MaxSummaryLength),
            UsedFallback = false
        };
    }

    private static ReportAnalysis Fallback(string description) => new()
    {
        CategoryId = null,
        Priority = Priority.Medium,
        Summary = Truncate(description, MaxSummaryLength),
        UsedFallback = true
    };

    private static string Truncate(string text, int maxLength) =>
        text.Length <= maxLength
            ? text
            : text[..(maxLength - 3)].TrimEnd() + "...";
}
