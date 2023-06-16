using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Demo.Redis.Infrastructure.Redis;

public class RedisConnectionFactory {
    private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> RedisConnections = new();

    public static async Task<ConnectionMultiplexer> GetConnection(string connectionString) {
        return RedisConnections.GetOrAdd(connectionString, await ConnectionMultiplexer.ConnectAsync(connectionString));
    }
}