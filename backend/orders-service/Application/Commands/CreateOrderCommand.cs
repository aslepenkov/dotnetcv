using MediatR;
using Newtonsoft.Json;
using OrdersService.Domain;

namespace OrdersService.Application.Commands;

public sealed record CreateOrderCommand(decimal Total, string Status) : IRequest<Order>;

public sealed record OrderCreated(Guid OrderId);

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly Infrastructure.OrderDbContext _context;

    public CreateOrderCommandHandler(Infrastructure.OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.Total, request.Status);

        await _context.Orders.AddAsync(order, cancellationToken);

        var orderCreatedEvent = new Outbox
        {
            Id = Guid.NewGuid(),
            Type = typeof(OrderCreated).AssemblyQualifiedName,
            Data = JsonConvert.SerializeObject(new OrderCreated(order.Id)),
            CreatedAt = DateTime.UtcNow
        };
        await _context.OutboxMessages.AddAsync(orderCreatedEvent, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return order;
    }
}