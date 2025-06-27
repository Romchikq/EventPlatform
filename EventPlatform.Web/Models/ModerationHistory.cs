using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlatform.Models
{
    public class ModerationHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int ModeratorId { get; set; }

        [Required]
        public string Action { get; set; }

        public string Comments { get; set; }

        [Required]
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [ForeignKey("ModeratorId")]
        public User Moderator { get; set; }
    }
}