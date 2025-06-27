using EventPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
public interface ITicketService
{
    Task<Ticket> CreateTicket(int eventId, int userId);
    Task CreateTicket(object eventId, int userId);
    Task<Ticket> GetTicketById(int ticketId);
    Task<Ticket> GetUserTicket(int ticketId, int userId);
    Task<IEnumerable<Ticket>> GetUserTickets(int userId);
    Task<bool> RequestRefund(int ticketId, int userId);
    Task<EventPlatform.Web.Models.TicketValidationResult> ValidateTicket(string qrData, int organizerId);
}