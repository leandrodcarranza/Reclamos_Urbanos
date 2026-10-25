using ReclamosUrbanos.Entities;

namespace ReclamosUrbanos.Presentation.DTOs;

public class CreateReportInput
{
    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int NeighborhoodId { get; set; }

    public Priority? Priority { get; set; }

    public string? Summary { get; set; }
}