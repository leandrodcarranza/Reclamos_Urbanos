
namespace ReclamosUrbanos.Presentation.DTOs;

public class UpdateReportInput
{
    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int NeighborhoodId { get; set; }
}
