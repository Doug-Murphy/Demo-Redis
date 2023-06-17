namespace Demo.Redis.Infrastructure.Postgres.Models;

/// <summary>
/// The record representing the PersonType table.
/// </summary>
public sealed record PersonTypeTable {
    /// <summary>The ID of the record.</summary>
    public int Id { get; init; }

    /// <summary>The value of the record.</summary>
    public required string Type { get; init; }
}