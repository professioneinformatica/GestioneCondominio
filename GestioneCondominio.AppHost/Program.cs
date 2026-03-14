var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure resources
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("gestionecondominio-postgres-data")
    .WithPgAdmin();

var condominioDb = postgres.AddDatabase("condominioDb");

var redis = builder.AddRedis("redis")
    .WithDataVolume("gestionecondominio-redis-data")
    .WithRedisCommander();

// Migration service — runs first, applies EF Core migrations
var migrationService = builder.AddProject<Projects.GestioneCondominio_MigrationService>("migration-service")
    .WithReference(condominioDb)
    .WaitFor(condominioDb);

// API service
var apiService = builder.AddProject<Projects.GestioneCondominio_ApiService>("api-service")
    .WithReference(condominioDb)
    .WithReference(redis)
    .WaitFor(condominioDb)
    .WaitFor(redis)
    .WaitForCompletion(migrationService);

// Blazor WebAssembly frontend
builder.AddProject<Projects.GestioneCondominio_Web>("web-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
