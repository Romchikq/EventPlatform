namespace EventPlatform.Models
{
    public class TicketValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public Ticket Ticket { get; set; }
    }
}