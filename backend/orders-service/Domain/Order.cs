namespace OrdersService.Domain;

public sealed record Order
{
    private Order() { } // For EF Core

    private Order(Guid id, decimal total, string status)
    {
        Id = id;
        Total = total;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; init; }
    public decimal Total { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public static Order Create(decimal total, string status)
    {
        if (total < 0)
            throw new ArgumentException("Total cannot be negative", nameof(total));

        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty", nameof(status));

        return new Order(Guid.NewGuid(), total, status);
    }

    public Order UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty", nameof(newStatus));

        return this with { Status = newStatus };
    }
}