var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("AuctionSystem-PostgresData")
    .WithPgAdmin();

var db = postgres.AddDatabase("AuctionSystem");

var api = builder.AddProject<Projects.AuctionSystem_Api>("api")
    .WithReference(db, connectionName: "Database")
    .WaitFor(db);


builder.Build().Run();