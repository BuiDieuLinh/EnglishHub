using EnglishHub.Application.DTOs;
using EnglishHub.Application.Interfaces;
using EnglishHub.Domain.Entities;
using EnglishHub.Domain.Enums;

namespace EnglishHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new LoginResponse
        {
            Token = GenerateToken(user),
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email is already in use.");

        var user = new Users
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student
        };

        await _userRepository.AddAsync(user);

        return new LoginResponse
        {
            Token = GenerateToken(user),
            Email = user.Email,
            FullName = user.FullName
        };
    }

    // Token generation is simplified for demo purposes. In production, use JWT or similar.
    private static string GenerateToken(Users user) =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}
