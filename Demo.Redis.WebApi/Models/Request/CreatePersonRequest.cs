using Demo.Redis.Infrastructure.Postgres.Enums;
using Demo.Redis.Infrastructure.Postgres.Models;
using System.ComponentModel.DataAnnotations;

namespace Demo.Redis.WebApi.Models.Request;

/// <summary>
/// The request structure for creating a person record.
/// </summary>
public sealed record CreatePersonRequest {
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public required PersonType Type { get; set; }

    public Person ToDatabasePerson() {
        return new Person {
            FirstName = FirstName,
            LastName = LastName,
            Type = Type
        };
    }
}