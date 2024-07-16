namespace WeatherSolution.Domain;

public class WeatherObservation
{
    public int Id { get; set; }
    public string LocationName { get; set; }
    public string ZipCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public string Conditions { get; set; }
    public DateTime ObservationDate { get; set; }
}

public enum Authorization
{
    Read,
    Write,
    Admin
}

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public Authorization Authorizations { get; set; }
    public string ImageUrl { get; set; }
}