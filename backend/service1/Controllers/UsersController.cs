using Microsoft.AspNetCore.Mvc;
using MediatR;
using Service1.Application.Commands;
using Service1.Domain;

namespace Service1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserCommand command)
    {
        var user = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        // TODO: Implement GetUserQuery
        return NotFound();
    }
}