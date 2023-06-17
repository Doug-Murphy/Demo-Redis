using Demo.Redis.Infrastructure.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Redis.Infrastructure.Postgres;

public sealed class PersonContext : DbContext {
    public PersonContext(DbContextOptions<PersonContext> options) : base(options) {
    }

    public DbSet<Person> Persons { get; set; }

    public DbSet<PersonTypeTable> PersonTypes { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        //seed person type data
        modelBuilder.Entity<PersonTypeTable>()
            .HasData(
                new PersonTypeTable {
                    Id = 1,
                    Type = "Student"
                },
                new PersonTypeTable {
                    Id = 2,
                    Type = "Teacher"
                });

        base.OnModelCreating(modelBuilder);
    }
}