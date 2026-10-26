using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOS;
using FinanceTracker.Api.Entities;
using FinanceTracker.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokens;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(AppDbContext db, ITokenService tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _db.Users.AnyAsync(u => u.Email == email))
                return Conflict(new { message = "Email is already registered." });

            var user = new User { FullName = dto.FullName.Trim(), Email = email };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(BuildResponse(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(BuildResponse(user));
        }

        private AuthResponseDto BuildResponse(User user)
        {
            var (token, expires) = _tokens.CreateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                ExpiresAt = expires
            };
        }
    }
}
