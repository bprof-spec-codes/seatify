using Api.Helpers;
using Data;
using Logic.Helper;
using Entities.Models;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add<ValidationFilter>();
                opt.Filters.Add<ExceptionFilter>();
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var databaseProvider = builder.Configuration["Database:Provider"];
            var databaseName = builder.Configuration["Database:Name"] ?? "SeatifyDb";

            var allowedOrigins = builder.Configuration
                .GetSection("AllowedOrigins")
                .Get<string[]>();

            // DI registrations
            builder.Services.AddTransient(typeof(Repository<>));
            builder.Services.AddSingleton<DtoProvider>();
            builder.Services.AddScoped<VenueService>();
            builder.Services.AddScoped<IAuditoriumService, AuditoriumService>();
            builder.Services.AddScoped<ISectorService, SectorService>();
            builder.Services.AddScoped<ISeatService, SeatService>();
            builder.Services.AddScoped<IOrganizerService, OrganizerService>();
            builder.Services.AddScoped<IEventOccurrenceService, EventOccurrenceService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<ILayoutMatrixService, LayoutMatrixService>();
            builder.Services.AddScoped<Logic.Services.IEventService, EventService>();
            builder.Services.AddScoped<ISeatOverrideService, SeatOverrideService>();
            builder.Services.AddScoped<BookingService>();
            builder.Services.AddScoped<QrService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", policy =>
                {
                    policy
                    .WithOrigins(allowedOrigins!)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

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

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles(); // Allow to serve /images/...

            app.UseCors("AllowOrigin");

            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbSeeder.Seed(db);
            }

            app.Run();
        }
    }
}
