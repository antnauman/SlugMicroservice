using System.Data;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using SlugMicroservice.Respositories;
using Npgsql;
using Xunit;
namespace SlugMicroservice.Tests;

public class DbIntegrationTests
{
    private readonly string ConnectionStringName = "SlugDb";
    private readonly string ConnectionString;
    private readonly IConfiguration Configuration;
    public DbIntegrationTests()
    {
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        ConnectionString = Configuration.GetConnectionString(ConnectionStringName);
    }

    [Fact]
    public void TestConnectionString()
    {
        Assert.False(String.IsNullOrWhiteSpace(ConnectionString));
    }

    [Fact]
    public async Task Can_Open_Db_Connection()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        Assert.Equal(ConnectionState.Open, connection.State);

        await connection.CloseAsync();
        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    [Fact]
    public async Task Can_Connect_To_Db()
    {
        var linkRepository = new LinkRepository(Configuration);
        var result = await linkRepository.PingDb();
        Assert.True(result);
    }
}
