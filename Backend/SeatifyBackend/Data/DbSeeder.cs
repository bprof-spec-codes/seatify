using Entities.Models;

namespace Data
{
    public class DbSeeder
    {
        public static void Seed(AppDbContext ctx)
        {
            if (!ctx.Events.Any())
            {
                var events = new List<Event>
                    {
                        new Event
                        {
                            Id = Guid.NewGuid(),
                            Name = "Teszt esemény 1",
                            Description = "Első seed esemény",
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
                    };

                ctx.Events.AddRange(events);
                ctx.SaveChanges();
            }
        }
    }
}
