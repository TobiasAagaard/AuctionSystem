# AuctionSystem

## Start the application with Aspire

Make sure Docker is running, then run this from the repository root:

```sh
dotnet run --project AppHost/AppHost.csproj
```

Aspire starts PostgreSQL and the API. The API waits for the database, receives its connection string from Aspire, and runs its database migrations on startup in the Development environment. The PostgreSQL data is persisted in a Docker volume, so it is retained when you stop and restart Aspire.

Open the Aspire Dashboard using the URL shown in the terminal to view the resources and their status. Stop the application with `Ctrl+C`.

You do not need to start the database separately with Docker Compose when using Aspire.

## Start Database for Avalonia project

``` 
// first time setup
docker compose up -d --build

// use afterwards
docker compose up -d
```

## Close Database

```
docker compose down

// if you want to delete the data as well
docker compose down -v
```
