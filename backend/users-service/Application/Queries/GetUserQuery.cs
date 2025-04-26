using MediatR;
using Microsoft.EntityFrameworkCore;
using UsersService.Domain;

namespace UsersService.Application.Queries;

public record GetUserQuery(Guid Id) : IRequest<User?>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User?>
{
    private readonly Infrastructure.UserDbContext _context;

    public GetUserQueryHandler(Infrastructure.UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
    }
}