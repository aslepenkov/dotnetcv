using MediatR;
using Service1.Domain;

namespace Service1.Application.Commands;

public record CreateUserCommand(string Email, string Name) : IRequest<User>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly Infrastructure.UserDbContext _context;
    private readonly Amazon.SimpleNotificationService.IAmazonSimpleNotificationService _sns;

    public CreateUserCommandHandler(
        Infrastructure.UserDbContext context,
        Amazon.SimpleNotificationService.IAmazonSimpleNotificationService sns)
    {
        _context = context;
        _sns = sns;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish user created event to SNS
        await _sns.PublishAsync(
            "arn:aws:sns:us-east-1:000000000000:user-events",
            System.Text.Json.JsonSerializer.Serialize(new { 
                EventType = "UserCreated",
                User = user
            }),
            cancellationToken);

        return user;
    }
}