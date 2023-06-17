using Demo.Redis.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddDbContext<PersonContext>(options => { options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")); });
builder.Services.AddScoped<PersonsRepository>();
builder.Services.AddSingleton<IConnectionMultiplexer>(x => {
    var connectionConfigOptions = new ConfigurationOptions {
        EndPoints = {
            builder.Configuration.GetConnectionString("Redis")!
        }
    };

    return ConnectionMultiplexer.Connect(connectionConfigOptions);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope()) {
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<PersonContext>();
    await dbContext.Database.MigrateAsync();
    //the DB is now seeded with the lookup data that is a good candidate for storing into Redis

    //fetch the data from the database
    var personTypes = dbContext.PersonTypes.AsNoTracking().Select(personType => new HashEntry(personType.Type, personType.Id)).ToArray();

    //store the data into Redis for future reads
    var redisConnection = serviceScope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
    await redisConnection.GetDatabase().HashSetAsync("PersonTypes", personTypes);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();