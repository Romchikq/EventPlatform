// UserService.cs
public class ChangePasswordDto
{

}
// ChangePasswordDto.cs
namespace EventPlatform.DTOs
{
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}