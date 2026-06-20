using EnglishHub.Application.DTOs;
using EnglishHub.Application.Interfaces;
using EnglishHub.Domain.Entities;
using EnglishHub.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnglishHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtOptions _jwtOptions;

    public AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildLoginResponse(user);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var existing = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existing is not null)
            throw new InvalidOperationException("Email is already in use.");

        var user = new Users
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            FullName = request.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student
        };

        await _userRepository.AddAsync(user);

        return BuildLoginResponse(user);
    }

    private LoginResponse BuildLoginResponse(Users user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

        return new LoginResponse
        {
            Token = GenerateToken(user, expiresAtUtc),
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            ExpiresAtUtc = expiresAtUtc
        };
    }

    private string GenerateToken(Users user, DateTime expiresAtUtc)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
