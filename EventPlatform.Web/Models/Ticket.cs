using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlatform.Models
{
    public enum TicketStatus
    {
        Active,
        Used,
        Refunded,
        Cancelled
    }

    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string TicketNumber { get; set; }

        [Required]
        public decimal PricePaid { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Active;

        public DateTime? UsedDate { get; set; }

        public DateTime? RefundDate { get; set; }

        public string PaymentId { get; set; }

        public string RefundId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }


    }


}