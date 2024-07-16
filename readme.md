```
```WeatherSolution
│
├───WeatherSolution.API
│   ├───Controllers
│   │   ├───WeatherObservationsController.cs
│   │   └───UsersController.cs
│   ├───Program.cs
│   └───Startup.cs
│
├───WeatherSolution.Application
│   ├───Commands
│   │   ├───AddObservationCommand.cs
│   │   └───AddUserCommand.cs
│   └───Queries
│       ├───GetObservationsQuery.cs
│       └───GetUsersQuery.cs
│
├───WeatherSolution.Domain
│   ├───Entities
│   │   ├───WeatherObservation.cs
│   │   └───User.cs
│   └───Enums
│       └───Authorization.cs
│
└───WeatherSolution.Infrastructure
    ├───DbContexts
    │   ├───WeatherDbContext.cs
    │   └───UserDbContext.cs
    └───Migrations
        ├───CreateWeatherObservationTable.cs
        └───CreateUserTable.cs
```