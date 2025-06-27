// UserService.cs
public class UserProfileUpdateDto
{
    public string? FirstName { get; internal set; }
    public string? LastName { get; internal set; }
}
// UserProfileUpdateDto.cs
namespace EventPlatform.DTOs
{
    public class UserProfileUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Другие поля профиля
    }
}

