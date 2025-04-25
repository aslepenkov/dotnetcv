namespace Service2.Domain;

public sealed record Order
{
    private Order() { } // For EF Core

    private Order(Guid id, Guid userId, decimal total, string status)
    {
        Id = id;
        UserId = userId;
        Total = total;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public decimal Total { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public static Order Create(Guid userId, decimal total)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user ID", nameof(userId));
        
        if (total < 0)
            throw new ArgumentException("Total cannot be negative", nameof(total));

        return new Order(Guid.NewGuid(), userId, total, "Pending");
    }

    public Order UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty", nameof(newStatus));

        return this with { Status = newStatus };
    }
}