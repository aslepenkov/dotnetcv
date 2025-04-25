using MediatR;
using Microsoft.EntityFrameworkCore;
using Service1.Domain;

namespace Service1.Application.Queries;

public record GetUsersQuery : IRequest<IEnumerable<User>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<User>>
{
    private readonly Infrastructure.UserDbContext _context;

    public GetUsersQueryHandler(Infrastructure.UserDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }
}