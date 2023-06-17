using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Demo.Redis.WebApi.Controllers;

/// <summary>
/// Endpoints for interacting with Redis.
/// </summary>
[Route("[controller]")]
public sealed class RedisController : ControllerBase {
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisController(IConnectionMultiplexer connectionMultiplexer) {
        _connectionMultiplexer = connectionMultiplexer;
    }

    /// <summary>
    /// Ping the Redis connection
    /// </summary>
    /// <returns>The amount of time that the ping took.</returns>
    [HttpGet]
    public async Task<IActionResult> Ping() {
        return Ok(await _connectionMultiplexer.GetDatabase().PingAsync());
    }
}