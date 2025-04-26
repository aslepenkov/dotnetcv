using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain;

namespace OrdersService.Application.Queries;

public record GetOrderQuery(Guid Id) : IRequest<Order?>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Order?>
{
    private readonly Infrastructure.OrderDbContext _context;

    public GetOrderQueryHandler(Infrastructure.OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
    }
}