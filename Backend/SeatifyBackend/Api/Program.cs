
using Data;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var databaseProvider = builder.Configuration["Database:Provider"];
            var databaseName = builder.Configuration["Database:Name"] ?? "SeatifyDb";

            if (string.Equals(databaseProvider, "InMemory", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName));
            }
            else
            {
                throw new InvalidOperationException("Unsupported database provider configured.");
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!dbContext.Events.Any())
                {
                    dbContext.Events.AddRange(
                        new Event
                        {
                            Id = Guid.NewGuid(),
                            Name = "Teszt esemény 1",
                            Description = "Elsõ seed esemény",
                            StartsAt = DateTime.UtcNow.AddDays(7),
                            EndsAt = DateTime.UtcNow.AddDays(7).AddHours(2),
                            BasePrice = 4990
                        },
                        new Event
                        {
                            Id = Guid.NewGuid(),
                            Name = "Teszt esemény 2",
                            Description = "Második seed esemény",
                            StartsAt = DateTime.UtcNow.AddDays(14),
                            EndsAt = DateTime.UtcNow.AddDays(14).AddHours(3),
                            BasePrice = 6990
                        }
                    );

                    dbContext.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
