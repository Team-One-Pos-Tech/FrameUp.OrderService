using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Npgsql;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace FrameUp.OrderService.Behaviour.Tests.Fixtures;

public class PostgresqlFixture
{
    private IContainer? _pgSqlContainer;

    public const int PgSQLPublicPort = 5432;
    public const string PgSQLUser = "postgres";
    public const string PgSQLPassword = "postgres";

    public async Task BaseSetUp()
    {
        _pgSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("order")
            .WithEnvironment("POSTGRES_USER", PgSQLUser)
            .WithEnvironment("POSTGRES_PASSWORD", PgSQLPassword)
            .WithPortBinding(5432)
            .WithCleanUp(true)
            .WithPortBinding(PgSQLPublicPort, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                .UntilPortIsAvailable(PgSQLPublicPort))
            .Build();

        await _pgSqlContainer.StartAsync();

        var connectionSring = $"Host=localhost;Port={_pgSqlContainer.GetMappedPublicPort(PgSQLPublicPort)};Database=postgres;Username={PgSQLUser};Password={PgSQLPassword}";

        var pgSqlClient = new NpgsqlConnection(connectionSring);

        await pgSqlClient.OpenAsync();
    }

    public async Task BaseTearDown()
    {
        await _pgSqlContainer!.StopAsync();
        await _pgSqlContainer.DisposeAsync();
    }
}
