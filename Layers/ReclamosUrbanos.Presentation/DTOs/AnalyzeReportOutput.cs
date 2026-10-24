using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.Presentation.DTOs;

public class AnalyzeReportOutput
{
    public int? CategoryId { get; set; }

    public Priority Priority { get; set; }

    public string Summary { get; set; } = string.Empty;

    public bool UsedFallback { get; set; }
}
