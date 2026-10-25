using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.BusinessLogic.Services;

/// <summary>Sugerencia de la IA para un reclamo. El usuario la confirma o la corrige.</summary>
public class ReportAnalysis
{
    /// <summary>Id de una categoría existente, o null si no se pudo determinar.</summary>
    public int? CategoryId { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;

    public string Summary { get; set; } = string.Empty;

    /// <summary>True si la IA falló y se devolvió el plan B.</summary>
    public bool UsedFallback { get; set; }

    public bool IsEmergency { get; set; }
}
