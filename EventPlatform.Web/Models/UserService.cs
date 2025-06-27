// UserService.cs
using EventPlatform.Data;
using EventPlatform.Models;

public class UserService : IUserService  // ← Важно добавить ": IUserService"
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> ChangePassword(int userId, ChangePasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> UpdateUserProfile(int userId, UserProfileUpdateDto updateDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        // Обновляем поля
        user.FirstName = updateDto.FirstName ?? user.FirstName;
        user.LastName = updateDto.LastName ?? user.LastName;
        // ... другие поля

        await _context.SaveChangesAsync();
        return user;
    }

    // Реализуйте все остальные методы интерфейса
    // ...
}