using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace EventPlatform.Models
{
    public enum EventStatus
    {
        Draft,
        OnModeration,
        Approved,
        Rejected,
        Cancelled,
        Completed
    }

    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganizerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string EventType { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Required]
        public string Location { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int TotalTickets { get; set; }

        public int AvailableTickets { get; set; }

        [Required]
        public EventStatus Status { get; set; } = EventStatus.Draft;

        public string RejectionReason { get; set; }

        public string Tags { get; set; }

        public string Mood { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("OrganizerId")]
        public User Organizer { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}