using Data;
using Entities.Dtos.Auth;
using Entities.Models;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly PasswordHasher<Organizer> _passwordHasher;

        public AuthService(AppDbContext dbContext, IJwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = new PasswordHasher<Organizer>();
        }

        public async Task<AuthResponseDto> LoginAsync(OrganizerLoginDto dto, CancellationToken ct)
        {
            if (dto == null)
            {
                throw new ArgumentException("Request body is required.");
            }

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

            var organizer = await _dbContext.Organizers.FirstOrDefaultAsync(o => o.Email.ToLower() == normalizedEmail, ct);

            if (organizer == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(
                organizer,
                organizer.PasswordHash,
                dto.Password
            );

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = _jwtTokenService.GenerateToken(organizer);
            var expiresAtUtc = _jwtTokenService.GetExpiryUtc();

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                OrganizerId = organizer.Id,
                Email = organizer.Email,
                Name = organizer.Name
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(OrganizerRegisterDto dto, CancellationToken ct)
        {
            if (dto == null)
            {
                throw new ArgumentException("Request body is required.");
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                throw new ArgumentException("Password confirmation does not match the password.");
            }

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

            var emailExists = await _dbContext.Organizers.AnyAsync(o => o.Email.ToLower() == normalizedEmail, ct);

            if (emailExists)
            {
                throw new ArgumentException("An organizer with this email already exists.");
            }

            var organizer = new Organizer
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email.Trim(),
                Name = dto.Name.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            organizer.PasswordHash = _passwordHasher.HashPassword(organizer, dto.Password);

            _dbContext.Organizers.Add(organizer);
            await _dbContext.SaveChangesAsync(ct);

            var token = _jwtTokenService.GenerateToken(organizer);
            var expiresAtUtc = _jwtTokenService.GetExpiryUtc();

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                OrganizerId = organizer.Id,
                Email = organizer.Email,
                Name = organizer.Name
            };
        }
    }
}
