using EventPlatform.Data;
using EventPlatform.Models;
using EventPlatform.Services;
using EventPlatform.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventPlatform.Web.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly IPaymentService _paymentService;

        public TicketService(AppDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<Ticket> CreateTicket(int eventId, int userId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            var user = await _context.Users.FindAsync(userId);

            if (eventItem == null || user == null)
                throw new ArgumentException("Invalid event or user");

            var ticket = new Ticket
            {
                EventId = eventId,
                UserId = userId,
                PurchaseDate = DateTime.UtcNow,
                Status = TicketStatus.Active
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public Task CreateTicket(object eventId, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Ticket> GetTicketById(int ticketId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<Ticket> GetUserTicket(int ticketId, int userId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.UserId == userId);
        }

        public async Task<IEnumerable<Ticket>> GetUserTickets(int userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.Event)
                .ToListAsync();
        }

        public async Task<bool> RequestRefund(int ticketId, int userId)
        {
            var ticket = await GetUserTicket(ticketId, userId);
            if (ticket == null || ticket.Status != TicketStatus.Active)
                return false;

            // Логика возврата средств
            return true;
        }

        public async Task<EventPlatform.Models.TicketValidationResult> ValidateTicket(string qrData, int organizerId)
        {
            // Реализация проверки QR-кода
            return new EventPlatform.Models.TicketValidationResult { IsValid = true };
        }

        Task<Models.TicketValidationResult> ITicketService.ValidateTicket(string qrData, int organizerId)
        {
            throw new NotImplementedException();
        }
    }
}