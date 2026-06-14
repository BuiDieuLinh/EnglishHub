using EnglishHub.Application.DTOs;
using EnglishHub.Domain.Entities;

namespace EnglishHub.Application.Interfaces;

public interface IUserRepository
{
    Task<Users?> GetByEmailAsync(string email);
    Task AddAsync(Users user);
}