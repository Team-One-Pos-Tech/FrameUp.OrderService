using DotNet.Testcontainers.Builders;
using Npgsql;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace FrameUp.OrderService.Behaviour.Tests.Fixtures;

public class PostgresqlFixture
{
    private PostgreSqlContainer? _pgSqlContainer = null;

    public const int PgSQLPublicPort = 5432;

    public const string PgSQLUser = "postgres";

    public const string PgSQLPassword = "postgres";
    public string ConnectionString { get; private set; }

    public async Task BaseSetUp()
    {
        _pgSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("FrameUpIntegrationTests")
            .WithUsername(PgSQLUser)
            .WithPassword(PgSQLPassword)
            .WithPortBinding(PgSQLPublicPort, true)
            .WithCleanUp(true)
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(PgSQLPublicPort))
            .Build();

        await _pgSqlContainer!.StartAsync();

        ConnectionString = _pgSqlContainer.GetConnectionString();
        var dataSourceBuilder = new NpgsqlDataSourceBuilder
        (
            ConnectionString
        );

        var dataSource = dataSourceBuilder.EnableDynamicJson().Build();
    }

    public async Task BaseTearDown()
    { 
        await _pgSqlContainer!.DisposeAsync();
    }
}
