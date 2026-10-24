namespace ReclamosUrbanos.Entities;

public class Report
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public int NeighborhoodId { get; set; }

    public Neighborhood? Neighborhood { get; set; }

    public Status Status { get; set; } = Status.Pending;

    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
