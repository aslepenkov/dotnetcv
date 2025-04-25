using System.Text.Json;
using Amazon.SimpleNotificationService;
using MediatR;
using Service1.Domain;

namespace Service1.Application.Commands;

public sealed record CreateUserCommand(string Email, string Name) : IRequest<User>;

public sealed record UserCreatedEvent(string EventType, User User);

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly Infrastructure.UserDbContext _context;
    private readonly IAmazonSimpleNotificationService _sns;

    public CreateUserCommandHandler(
        Infrastructure.UserDbContext context,
        IAmazonSimpleNotificationService sns)
    {
        _context = context;
        _sns = sns;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Domain validation and creation using factory method
        var user = User.Create(request.Email, request.Name);

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Using records for event data
        var userEvent = new UserCreatedEvent("UserCreated", user);
        await _sns.PublishAsync(
            "arn:aws:sns:us-east-1:000000000000:user-events",
            JsonSerializer.Serialize(userEvent),
            cancellationToken);

        return user;
    }
}