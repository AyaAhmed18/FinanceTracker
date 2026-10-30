using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinanceTracker.Api.Entities;
using FinanceTracker.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.Tests;

public class TokenServiceTests
{
    private static TokenService CreateService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestKey_AtLeast32Characters_Long_123456",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:ExpiryMinutes"] = "30"
            })
            .Build();
        return new TokenService(config);
    }

    [Fact]
    public void CreateToken_ContainsUserClaims()
    {
        var user = new User { Id = 7, FullName = "Aya", Email = "aya@test.com" };

        var (token, _) = CreateService().CreateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.Should().Be("7");
        jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value.Should().Be("aya@test.com");
    }

    [Fact]
    public void CreateToken_ExpiresInConfiguredTime()
    {
        var user = new User { Id = 1, FullName = "A", Email = "a@test.com" };

        var (_, expires) = CreateService().CreateToken(user);

        expires.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromMinutes(1));
    }
}