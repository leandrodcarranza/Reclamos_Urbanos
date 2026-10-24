
using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.Presentation.DTOs;

public class ReportOutput
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public int CategoryId { get; set; }

    public int NeighborhoodId { get; set; }

    public Status Status { get; set; }

    public Priority Priority { get; set; }

    public DateTime CreatedAt { get; set; }
}
