var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure resources
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("gestionecondominio-postgres-data")
    .WithPgAdmin();

var condominioDb = postgres.AddDatabase("condominioDb");

var redis = builder.AddRedis("redis")
    .WithDataVolume("gestionecondominio-redis-data")
    .WithRedisCommander();

var vaultDevRootToken = builder.AddParameter("vault-root-token", secret: true);

var vault = builder.AddContainer("vault", "hashicorp/vault", "latest")
    .WithVolume("gestionecondominio-vault-data", "/vault/data")
    .WithHttpEndpoint(port: 8200, targetPort: 8200, name: "vault-api")
    .WithEnvironment("VAULT_DEV_ROOT_TOKEN_ID", vaultDevRootToken)
    .WithEnvironment("VAULT_DEV_LISTEN_ADDRESS", "0.0.0.0:8200")
    .WithArgs("server", "-dev");

// Migration service — runs first, applies EF Core migrations
var migrationService = builder.AddProject<Projects.GestioneCondominio_MigrationService>("migration-service")
    .WithReference(condominioDb)
    .WaitFor(condominioDb);

// API service
var apiService = builder.AddProject<Projects.GestioneCondominio_ApiService>("api-service")
    .WithReference(condominioDb)
    .WithReference(redis)
    .WithEnvironment("VAULT__URL", vault.GetEndpoint("vault-api"))
    .WithEnvironment("VAULT__TOKEN", vaultDevRootToken)
    .WaitFor(condominioDb)
    .WaitFor(redis)
    .WaitFor(vault)
    .WaitForCompletion(migrationService);

// Blazor WebAssembly frontend
builder.AddProject<Projects.GestioneCondominio_Web>("web-frontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
