public class GetObservationsQueryHandler : IRequestHandler<GetObservationsQuery, IEnumerable<WeatherObservation>>
{
    private readonly WeatherDbContext _context;

    public GetObservationsQueryHandler(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WeatherObservation>> Handle(GetObservationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.WeatherObservations.ToListAsync(cancellationToken);
    }
}