using EventPlatform.Data;
using EventPlatform.Models;
using EventPlatform.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventPlatform.Services
{
    public class ModerationService : IModerationService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ModerationService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> ApproveEvent(int eventId, int moderatorId, string comments)
        {
            var eventToApprove = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventToApprove == null) return false;

            eventToApprove.Status = EventStatus.Approved;

            var history = new ModerationHistory
            {
                EventId = eventId,
                ModeratorId = moderatorId,
                Action = "Approved",
                Comments = comments,
                ActionDate = DateTime.UtcNow
            };

            _context.ModerationHistories.Add(history);
            await _context.SaveChangesAsync();

            await _emailService.SendEventApprovalNotification(
                eventToApprove.Organizer.Email,
                eventToApprove.Title,
                true,
                comments);

            return true;
        }

        public async Task<bool> RejectEvent(int eventId, int moderatorId, string reason)
        {
            var eventToReject = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventToReject == null) return false;

            eventToReject.Status = EventStatus.Rejected;

            var history = new ModerationHistory
            {
                EventId = eventId,
                ModeratorId = moderatorId,
                Action = "Rejected",
                Comments = reason,
                ActionDate = DateTime.UtcNow
            };

            _context.ModerationHistories.Add(history);
            await _context.SaveChangesAsync();

            await _emailService.SendEventApprovalNotification(
                eventToReject.Organizer.Email,
                eventToReject.Title,
                false,
                reason);

            return true;
        }

        public async Task<IEnumerable<Event>> GetEventsForModeration()
        {
            return await _context.Events
                .Where(e => e.Status == EventStatus.OnModeration)
                .Include(e => e.Organizer)
                .ToListAsync();
        }

        public async Task<ModerationHistory> GetModerationHistory(int eventId)
        {
            return await _context.ModerationHistories
                .Include(m => m.Moderator)
                .Where(m => m.EventId == eventId)
                .OrderByDescending(m => m.ActionDate)
                .FirstOrDefaultAsync();
        }
    }
}