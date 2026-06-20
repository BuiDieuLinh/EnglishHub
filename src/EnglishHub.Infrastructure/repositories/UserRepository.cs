using Microsoft.EntityFrameworkCore;
using EnglishHub.Application.Interfaces;
using EnglishHub.Domain.Entities;
using EnglishHub.Infrastructure.Persistence;

namespace EnglishHub.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Users?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task AddAsync(Users user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
