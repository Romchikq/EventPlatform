using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace EventPlatform.Models
{
    public enum UserRole
    {
        Visitor,
        Organizer,
        Moderator,
        Admin
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Visitor;

        public bool IsEmailConfirmed { get; set; } = false;

        public string EmailConfirmationToken { get; set; }

        public DateTime? EmailConfirmationTokenExpiry { get; set; }

        public string PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Event> OrganizedEvents { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}