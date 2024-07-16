[ApiController]
[Route("[controller]")]
public class WeatherObservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeatherObservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddObservation(AddObservationCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetObservations(GetObservationsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}