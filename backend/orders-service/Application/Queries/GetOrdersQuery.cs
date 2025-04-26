using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersService.Domain;

namespace OrdersService.Application.Queries;

public record GetOrdersQuery : IRequest<IEnumerable<Order>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<Order>>
{
    private readonly Infrastructure.OrderDbContext _context;

    public GetOrdersQueryHandler(Infrastructure.OrderDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders.ToListAsync(cancellationToken);
    }
}