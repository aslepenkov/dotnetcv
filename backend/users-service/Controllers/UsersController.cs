using Microsoft.AspNetCore.Mvc;
using MediatR;
using UsersService.Application.Commands;
using UsersService.Application.Queries;
using UsersService.Domain;

namespace UsersService.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Ok(users);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserCommand command)
    {
        try
        {
            var user = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _mediator.Send(new GetUserQuery(id));
        if (user == null)
            return NotFound(new { Message = $"User with ID {id} not found" });
        return Ok(user);
    }

    /// <summary>
    /// A synthetic bottleneck endpoint to simulate high CPU load for HPA testing.
    /// Enabled via ENABLE_BOTTLENECK environment variable.
    /// </summary>
    [HttpGet("bottleneck")]
    public IActionResult Bottleneck([FromQuery] int iterations = 100000000)
    {
        var isEnabled = Environment.GetEnvironmentVariable("ENABLE_BOTTLENECK") == "true";
        if (!isEnabled)
        {
            return BadRequest(new { Message = "Bottleneck simulation is not enabled. Set ENABLE_BOTTLENECK=true." });
        }

        // Perform some CPU intensive work
        double result = 0;
        for (int i = 0; i < iterations; i++)
        {
            result += Math.Sqrt(i);
        }

        return Ok(new { Message = "Bottleneck simulation completed", Result = result });
    }
}