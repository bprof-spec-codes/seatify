using Api.Helpers;
using Data;
using Logic.Helper;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Resend;

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
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Seatify API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header. Example: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var databaseProvider = builder.Configuration["Database:Provider"];
            var databaseName = builder.Configuration["Database:Name"] ?? "SeatifyDb";

            var allowedOrigins = builder.Configuration
                .GetSection("AllowedOrigins")
                .Get<string[]>();

            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");

            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");

            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");

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
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddOptions();
            builder.Services.AddHttpClient<ResendClient>();
            builder.Services.Configure<ResendClientOptions>(o =>
            {
                o.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_KEY") 
                    ?? builder.Configuration["Resend:ApiToken"] 
                    ?? string.Empty;
            });
            builder.Services.AddTransient<IResend, ResendClient>();

            builder.Services.AddScoped<ICheckInService, CheckInService>();
            builder.Services.AddScoped<QrService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

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

            app.UseAuthentication();
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
