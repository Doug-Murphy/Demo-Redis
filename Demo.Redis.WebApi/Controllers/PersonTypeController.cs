using Demo.Redis.Infrastructure.Postgres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Net;

namespace Demo.Redis.WebApi.Controllers;

/// <summary>
/// Endpoints for interacting with the PersonTypes table.
/// </summary>
[Route("[controller]")]
public class PersonTypeController : ControllerBase {
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly PersonContext _personContext;

    public PersonTypeController(IConnectionMultiplexer connectionMultiplexer, PersonContext personContext) {
        _connectionMultiplexer = connectionMultiplexer;
        _personContext = personContext;
    }

    /// <summary>
    /// Retrieve all person types available from Redis.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("from-redis")]
    [ProducesResponseType(typeof(Dictionary<string, int>), (int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAllFromRedis() {
        var personTypes = await _connectionMultiplexer.GetDatabase().HashGetAllAsync("PersonTypes");

        if (personTypes.Length == 0) {
            return NotFound();
        }

        return Ok(personTypes.ToDictionary(personType => personType.Name.ToString(), personType => (int) personType.Value));
    }

    /// <summary>
    /// Retrieve all person types available from the database.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("from-db")]
    [ProducesResponseType(typeof(Dictionary<string, int>), (int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAllFromDatabase() {
        var personTypes = await _personContext.PersonTypes.AsNoTracking().ToListAsync();

        if (personTypes.Count == 0) {
            return NotFound();
        }

        return Ok(personTypes.ToDictionary(personType => personType.Type, personType => personType.Id));
    }
}