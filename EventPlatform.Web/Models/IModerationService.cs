using EventPlatform.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventPlatform.Services
{
    public interface IModerationServices
    {
        Task<bool> ApproveEvent(int eventId, string moderatorComment);
        Task<bool> RejectEvent(int eventId, string rejectionReason);
        Task<List<Event>> GetPendingEvents();
        Task<ModerationHistory> GetModerationHistory(int eventId);
    }
}