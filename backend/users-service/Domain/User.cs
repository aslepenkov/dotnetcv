namespace UsersService.Domain;

public sealed record User
{
    private User() { } // For EF Core

    private User(Guid id, string email, string name)
    {
        Id = id;
        Email = email.ToLowerInvariant();
        Name = name;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsActive { get; init; }

    public static User Create(string email, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new User(Guid.NewGuid(), email, name);
    }

    public User Deactivate() => this with { IsActive = false };
}