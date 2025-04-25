using System.Text.Json;
using Amazon.SQS;
using MediatR;
using Service2.Domain;

namespace Service2.Application.Commands;

public sealed record CreateOrderCommand(Guid UserId, decimal Total) : IRequest<Order>;

public sealed record OrderCreatedEvent(string EventType, Order Order);

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly Infrastructure.OrderDbContext _context;
    private readonly IAmazonSQS _sqs;

    public CreateOrderCommandHandler(
        Infrastructure.OrderDbContext context,
        IAmazonSQS sqs)
    {
        _context = context;
        _sqs = sqs;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Domain validation and creation using factory method
        var order = Order.Create(request.UserId, request.Total);

        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Using records for event data
        var orderEvent = new OrderCreatedEvent("OrderCreated", order);
        await _sqs.SendMessageAsync(
            "http://dotnetcv-localstack:4566/000000000000/order-events",
            JsonSerializer.Serialize(orderEvent),
            cancellationToken);

        return order;
    }
}