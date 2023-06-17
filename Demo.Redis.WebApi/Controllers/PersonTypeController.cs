using Demo.Redis.Infrastructure.Postgres;
using Demo.Redis.Infrastructure.Postgres.Models;
using Demo.Redis.WebApi.Models.Request;
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

    /// <summary>
    /// Add a new person type value.
    /// </summary>
    /// <param name="request">The person type value to create.</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType((int) HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] CreatePersonTypeRequest request) {
        var newRecord = new PersonTypeTable {Type = request.Type};
        _personContext.PersonTypes.Add(newRecord);
        await _personContext.SaveChangesAsync();

        //after saving new record to database, refresh Redis with the latest values
        var personTypes = _personContext.PersonTypes.AsNoTracking().Select(personType => new HashEntry(personType.Type, personType.Id)).ToArray();
        await _connectionMultiplexer.GetDatabase().HashSetAsync("PersonTypes", personTypes);

        return new StatusCodeResult((int) HttpStatusCode.Created);
    }
}