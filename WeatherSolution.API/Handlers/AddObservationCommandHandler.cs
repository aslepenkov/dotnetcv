public class AddObservationCommandHandler : IRequestHandler<AddObservationCommand, WeatherObservation>
{
    private readonly WeatherDbContext _context;

    public AddObservationCommandHandler(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task<WeatherObservation> Handle(AddObservationCommand request, CancellationToken cancellationToken)
    {
        var observation = new WeatherObservation
        {
            LocationName = request.LocationName,
            ZipCode = request.ZipCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Temperature = request.Temperature,
            WindSpeed = request.WindSpeed,
            Conditions = request.Conditions,
            ObservationDate = request.ObservationDate
        };

        _context.WeatherObservations.Add(observation);
        await _context.SaveChangesAsync(cancellationToken);

        return observation;
    }
}