using ReclamosUrbanos.BusinessLogic.Services;
using ReclamosUrbanos.DataAccess.Stores;
using ReclamosUrbanos.Entities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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

        var isEmergency = analysis.IsEmergency || LooksLikeEmergency(description);

        return new ReportAnalysis
        {
            CategoryId = categoryId,
            Priority = priority,
            Summary = BuildSummary(summary, isEmergency),
            IsEmergency = isEmergency,
            UsedFallback = false
        };
    }

    private static ReportAnalysis Fallback(string description)
    {
        var isEmergency = LooksLikeEmergency(description);

        return new ReportAnalysis
        {
            CategoryId = null,
            Priority = Priority.Medium,
            Summary = BuildSummary(description, isEmergency),
            IsEmergency = isEmergency,
            UsedFallback = true
        };
    }

    private static string Truncate(string text, int maxLength) =>
        text.Length <= maxLength
            ? text
            : text[..(maxLength - 3)].TrimEnd() + "...";

    private const string EmergencyNotice = "Ante una emergencia, llamá al 911.";

    // Red de seguridad: detecta emergencias aunque la IA falle o no las marque.
    private static readonly Regex EmergencyPattern = new(
        @"\b(robos?|asaltos?|violencia|agresion(es)?|disparos?|balacera|tiroteo|incendio|herid[oa]s?|emergencia|secuestro|armas?|ambulancia|violacion|abuso|amenazas?)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static bool LooksLikeEmergency(string description) =>
        EmergencyPattern.IsMatch(RemoveAccents(description));

    private static string RemoveAccents(string text)
    {
        var decomposed = text.Normalize(NormalizationForm.FormD);
        var chars = decomposed.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        return new string(chars.ToArray()).Normalize(NormalizationForm.FormC);
    }

    private static string BuildSummary(string summary, bool isEmergency)
    {
        if (!isEmergency)
            return Truncate(summary, MaxSummaryLength);

        // El aviso se agrega por código y el total respeta el límite de 200 caracteres.
        var room = MaxSummaryLength - EmergencyNotice.Length - 1;
        return $"{Truncate(summary, room)} {EmergencyNotice}";
    }
}
