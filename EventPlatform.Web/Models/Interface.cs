using EventPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventPlatform.Web.Models  // Добавляем пространство имен
{
    public class TicketValidationResult  // Добавляем недостающий класс
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public Ticket Ticket { get; set; }
    }

    public interface ITicketService
    {
        Task<Ticket> CreateTicket(int eventId, int userId);
        Task<Ticket> GetUserTicket(int ticketId, int userId);
        Task<IEnumerable<Ticket>> GetUserTickets(int userId);
        Task<bool> RequestRefund(int ticketId, int userId);
        Task<TicketValidationResult> ValidateTicket(string qrData, int organizerId);
    }

    public interface IModerationService
    {
        Task<bool> ApproveEvent(int eventId, int moderatorId, string comments);
        Task<bool> RejectEvent(int eventId, int moderatorId, string reason);
        Task<IEnumerable<Event>> GetEventsForModeration();
    }

    public interface IUserService
    {
        Task<User> GetUserById(int id);
        Task<User> UpdateUserProfile(int userId, UserProfileUpdateDto updateDto);
        Task<bool> ChangePassword(int userId, ChangePasswordDto changePasswordDto);
    }
}