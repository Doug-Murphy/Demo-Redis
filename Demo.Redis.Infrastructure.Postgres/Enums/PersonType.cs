using System.Text.Json.Serialization;

namespace Demo.Redis.Infrastructure.Postgres.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PersonType {
    Unknown = 0,
    Student = 1,
    Teacher = 2
}