using System.Text.Json;
using Amazon.SimpleNotificationService;
using MediatR;
using OrdersService.Domain;

namespace OrdersService.Application.Commands;

public sealed record CreateOrderCommand(string Email, string Name) : IRequest<Order>;

public sealed record OrderCreatedEvent(string EventType, Order Order);

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly Infrastructure.OrderDbContext _context;
    private readonly IAmazonSimpleNotificationService _sns;

    public CreateOrderCommandHandler(
        Infrastructure.OrderDbContext context,
        IAmazonSimpleNotificationService sns)
    {
        _context = context;
        _sns = sns;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Domain validation and creation using factory method
        var Order = Order.Create(request.Email, request.Name);

        await _context.Orders.AddAsync(Order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Using records for event data
        var OrderEvent = new OrderCreatedEvent("OrderCreated", Order);
        await _sns.PublishAsync(
            "arn:aws:sns:us-east-1:000000000000:Order-events",
            JsonSerializer.Serialize(OrderEvent),
            cancellationToken);

        return Order;
    }
}