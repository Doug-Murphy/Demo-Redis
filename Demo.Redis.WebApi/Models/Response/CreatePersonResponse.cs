﻿using Demo.Redis.Infrastructure.Postgres.Enums;
using Demo.Redis.Infrastructure.Postgres.Models;

namespace Demo.Redis.WebApi.Models.Response;

/// <summary>
/// The API response when a person is created.
/// </summary>
public sealed record CreatePersonResponse {
    public CreatePersonResponse(Person dbPerson) {
        Id = dbPerson.Id;
        FirstName = dbPerson.FirstName;
        LastName = dbPerson.LastName;
        Type = dbPerson.Type;
    }

    public CreatePersonResponse() {
    }

    public Guid Id { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public PersonType Type { get; init; }
}