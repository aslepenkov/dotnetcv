using Microsoft.AspNetCore.Mvc;
using MediatR;
using OrdersService.Application.Commands;
using OrdersService.Application.Queries;
using OrdersService.Domain;

namespace OrdersService.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all Orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var Orders = await _mediator.Send(new GetOrdersQuery());
        return Ok(Orders);
    }

    /// <summary>
    /// Creates a new Order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        try
        {
            var Order = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = Order.Id }, Order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Gets a Order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        var Order = await _mediator.Send(new GetOrderQuery(id));
        if (Order == null)
            return NotFound(new { Message = $"Order with ID {id} not found" });
        return Ok(Order);
    }
}