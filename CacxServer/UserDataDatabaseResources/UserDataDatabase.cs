using CacxServer.Helper;
using CacxShared.Helper;
using CacxShared.SharedDTOs;
using Cristiano3120.Logging;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CacxServer.UserDataDatabaseResources;

public class UserDataDatabase
{
    private readonly string _connectionString;
    private readonly Logger _logger;

    public UserDataDatabase(Logger logger, string connStr)
    {
        _connectionString = connStr;
        _logger = logger;
    }

    public async Task WarmupAsync()
    {
        await WarmupDbPoolAsync();
        for (int i = 0; i < 100; i++)
        {
            _ = await CheckEmailAndUsernameAsync(new UniqueUserData() { Email = "TestEmail", Username = "TestUsername" });
        }
    }

    private async Task WarmupDbPoolAsync(int count = 5, int queriesPerConn = 1000)
    {
        _logger.LogInformation(LoggerParams.None, "Starting DB pool warmup...");
        Stopwatch stopwatch = Stopwatch.StartNew();

        List<NpgsqlConnection> conns = [];

        for (int i = 0; i < count; i++)
        {
            NpgsqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            conns.Add(conn);

            for (int q = 0; q < queriesPerConn; q++)
            {
                await using NpgsqlCommand cmd = new("SELECT 1;", conn);
                _ = await cmd.ExecuteScalarAsync();
            }
        }

        foreach (NpgsqlConnection conn in conns)
            await conn.CloseAsync();

        stopwatch.Stop();

        double totalQueries = count * queriesPerConn;
        double avgTimePerQueryMs = stopwatch.Elapsed.TotalMilliseconds / totalQueries;

        _logger.LogInformation(LoggerParams.None, $"Warmup finished. Total time: {stopwatch.Elapsed.TotalSeconds:F3}s");
        _logger.LogInformation(LoggerParams.None, $"Average time per query: {avgTimePerQueryMs:F4} ms");

        await Task.Delay(200);
    }

    /// <summary>
    /// Checks if the Email and/or the Username are already somewhere in the Users table
    /// </summary>
    /// <returns>A tuple that indicates which of the two was found. true == found | false == not found 
    /// | true, true indicates that something went wrong</returns>
    public async Task<DatabaseResult<(bool emailFound, bool usernameFound)>> CheckEmailAndUsernameAsync(UniqueUserData uniqueUserData)
    {
        try
        {
            await using NpgsqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            if (!string.IsNullOrEmpty(uniqueUserData.Username))
            {
                const string SqlUsername = $@"
                SELECT EXISTS (
                    SELECT 1
                    FROM ""{nameof(UserDataDbContext.Users)}""
                    WHERE ""{nameof(DbUser.Username)}"" = @Username
                );";

                await using (NpgsqlCommand cmd = new(SqlUsername, conn))
                {
                    _ = cmd.Parameters.AddWithValue("Username", uniqueUserData.Username);
                    if (await cmd.ExecuteScalarAsync() is bool usernameExists && usernameExists)
                        return new DatabaseResult<(bool emailFound, bool usernameFound)>
                        {
                            RequestSuccessful = true,
                            ReturnedValue = (emailFound: false, usernameFound: true)
                        };
                }
            }

            const string SqlEmail = $@"
            SELECT ""{nameof(DbUser.EmailHash)}""
            FROM ""{nameof(UserDataDbContext.Users)}"";";

            await using (NpgsqlCommand cmd = new(SqlEmail, conn))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    byte[] storedHash = await reader.GetFieldValueAsync<byte[]>(0);
                    if (SharedCryptographyHelper.Verify(Encoding.UTF8.GetBytes(uniqueUserData.Email!), storedHash))
                    {
                        return new DatabaseResult<(bool emailFound, bool usernameFound)>
                        {
                            RequestSuccessful = true,
                            ReturnedValue = (emailFound: true, usernameFound: false)
                        };
                    }
                }
            }

            return new DatabaseResult<(bool emailFound, bool usernameFound)>
            {
                RequestSuccessful = true,
                ReturnedValue = (emailFound: false, usernameFound: false)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggerParams.None, ex, CallerInfos.Create());
            return new DatabaseResult<(bool emailFound, bool usernameFound)>
            {
                RequestSuccessful = false,
                ReturnedValue = (emailFound: true, usernameFound: true)
            };
        }
    }
}
