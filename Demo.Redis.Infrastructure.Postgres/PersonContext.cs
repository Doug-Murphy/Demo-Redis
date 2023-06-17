using Demo.Redis.Infrastructure.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Redis.Infrastructure.Postgres;

public sealed class PersonContext : DbContext {
    public PersonContext(DbContextOptions<PersonContext> options) : base(options) {
    }

    public DbSet<Person> Persons { get; set; }
}