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
}