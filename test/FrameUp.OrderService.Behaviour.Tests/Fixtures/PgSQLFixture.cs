using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Npgsql;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FrameUp.OrderService.Behaviour.Tests.Fixtures;

public class PgSQLFixture
{
    private IContainer? _pgSqlContainer;

    public const int PgSQLPublicPort = 5432;
    public const string PgSQLUser = "postgres";
    public const string PgSQLPassword = "postgres";

    [SetUp]
    protected async Task BaseSetUp()
    {
        _pgSqlContainer = new ContainerBuilder()
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

    [TearDown]
    protected async Task BaseTearDown()
    {
        await _pgSqlContainer!.StopAsync();
        await _pgSqlContainer.DisposeAsync();
    }
}
