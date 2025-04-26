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


docker compose down && docker compose build --no-cache && docker compose up -d

docker exec -it dotnetcv-dev dotnet publish /app/lambda/PostgresHealthLambda/src/PostgresHealthLambda/PostgresHealthLambda.csproj -c Release -r linux-x64 --self-contained true -o /app/lambda/PostgresHealthLambda/src/PostgresHealthLambda/out

sudo chown -R $(whoami):$(whoami) ~/dev/dotnetcv/lambda

docker exec -it dotnetcv-dev dotnet publish -c Release -r linux-x64 --self-contained true -o out
zip -r function.zip .

docker exec -it dotnetcv-localstack bash /init-localstack.sh

docker exec -it dotnetcv-localstack awslocal lambda invoke \
  --function-name PostgresHealthLambda \
  --payload '{}' /dev/stdout


  WIPE

  docker ps -aq | xargs -r docker rm -f && docker images -aq | xargs -r docker rmi -f && docker volume ls -q | xargs -r docker volume rm
