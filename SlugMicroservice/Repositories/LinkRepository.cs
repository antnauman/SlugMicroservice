using Npgsql;
using Dapper;
using System.Data;
using SlugMicroservice.Models;
using System.Data.Common;

public interface ILinkRepository
{
    Task<bool> CreateLink(string targetUrl, string code);
    Task<Link> GetLinkById(string id);
    Task<Link> GetLinkByCode(string code);
}

public class LinkRepository : ILinkRepository
{
    private string ConnectionString { get; set; }

    public LinkRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("SlugDb");
    }

    public async Task<bool> PingDb()
    {
        var query = "SELECT 1";
        using var dbConnection = new NpgsqlConnection(ConnectionString);
        try
        {
            var result = await dbConnection.QueryFirstOrDefaultAsync<int>(query);
            return true;
        }
        catch (Exception ex)
        { throw; }
    }

    public async Task<bool> CreateLink(string targetUrl, string code)
    {
        var query = "INSERT INTO public.\"Links\" (\"TargetUrl\", \"Code\") VALUES (@TargetUrl, @Code) RETURNING \"Id\"";
        using var dbConnection = new NpgsqlConnection(ConnectionString);
        var idGuid = await dbConnection.ExecuteScalarAsync<Guid>(query, new { TargetUrl = targetUrl, Code = code });
        return (idGuid != Guid.Empty);
    }

    public async Task<Link> GetLinkById(string id)
    {
        var query = "SELECT \"Id\", \"Code\", \"TargetUrl\", \"CreatedUtc\" FROM public.\"Links\" WHERE \"Id\" = @Id";
        using var dbConnection = new NpgsqlConnection(ConnectionString);
        var link = await dbConnection.QueryFirstOrDefaultAsync<Link>(query, new { Id = id });
        return link;
    }

    public async Task<Link> GetLinkByCode(string code)
    {
        var query = "SELECT \"Id\", \"Code\", \"TargetUrl\", \"CreatedUtc\" FROM public.\"Links\" WHERE \"Code\" = @Code";
        using var dbConnection = new NpgsqlConnection(ConnectionString);
        var link = await dbConnection.QueryFirstOrDefaultAsync<Link>(query, new { Code = code });
        return link;
    }
}