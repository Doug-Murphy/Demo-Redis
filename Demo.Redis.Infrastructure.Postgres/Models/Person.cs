using System.ComponentModel.DataAnnotations;

namespace Demo.Redis.Infrastructure.Postgres.Models;

public record Person {
    public Guid Id { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }
}