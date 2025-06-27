// IUserService.cs
using EventPlatform.Models;

public interface IUserService
{
    Task<User> GetUserById(int id);
    Task<User> UpdateUserProfile(int userId, UserProfileUpdateDto updateDto);
    Task<bool> ChangePassword(int userId, ChangePasswordDto changePasswordDto);
    Task<IEnumerable<User>> GetAllUsers();
    Task<bool> DeleteUser(int userId);
}