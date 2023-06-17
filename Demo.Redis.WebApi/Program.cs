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

builder.Services.AddDbContext<PersonContext>();
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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();