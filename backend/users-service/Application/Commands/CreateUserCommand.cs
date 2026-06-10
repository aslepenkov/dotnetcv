using MediatR;
using Newtonsoft.Json;
using UsersService.Domain;

namespace UsersService.Application.Commands;

public sealed record CreateUserCommand(string Email) : IRequest<User>;

public sealed record UserCreated(Guid UserId, string Email);

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly Infrastructure.UserDbContext _context;

    public CreateUserCommandHandler(Infrastructure.UserDbContext context)
    {
        _context = context;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.Email, "New User");

        await _context.Users.AddAsync(user, cancellationToken);

        var userCreatedEvent = new Outbox
        {
            Id = Guid.NewGuid(),
            Type = typeof(UserCreated).AssemblyQualifiedName,
            Data = JsonConvert.SerializeObject(new UserCreated(user.Id, user.Email)),
            CreatedAt = DateTime.UtcNow
        };
        await _context.OutboxMessages.AddAsync(userCreatedEvent, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }
}