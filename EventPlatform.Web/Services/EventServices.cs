using EventPlatform.Data;
using EventPlatform.DTOs;
using EventPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Services
{
    public interface IEventService
    {
        Task<Event> CreateEvent(EventCreateDto eventDto, int organizerId);
        Task<Event> GetEvent(int id);
        Task<IEnumerable<Event>> GetEvents(EventFilterDto filter);
        Task<Event> UpdateEvent(int eventId, EventUpdateDto eventDto, int organizerId);
        Task<bool> DeleteEvent(int eventId, int organizerId);
        Task<IEnumerable<Event>> GetOrganizerEvents(int organizerId);
        Task<IEnumerable<EventType>> GetEventTypes();
    }

    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly IMapService _mapService;

        public EventService(AppDbContext context, IMapService mapService)
        {
            _context = context;
            _mapService = mapService;
        }

        public async Task<Event> CreateEvent(EventCreateDto eventDto, int organizerId)
        {
            var organizer = await _context.Users.FindAsync(organizerId);

            if (organizer == null || organizer.Role != UserRole.Organizer)
            {
                throw new Exception("Only organizers can create events");
            }

            var (latitude, longitude) = await _mapService.GetCoordinates(eventDto.Location);

            var newEvent = new Event
            {
                OrganizerId = organizerId,
                Title = eventDto.Title,
                Description = eventDto.Description,
                EventType = eventDto.EventType,
                StartDateTime = eventDto.StartDateTime,
                EndDateTime = eventDto.EndDateTime,
                Location = eventDto.Location,
                Latitude = latitude,
                Longitude = longitude,
                Price = eventDto.Price,
                TotalTickets = eventDto.TotalTickets,
                AvailableTickets = eventDto.TotalTickets,
                Status = EventStatus.OnModeration,
                Tags = string.Join(",", eventDto.Tags),
                Mood = eventDto.Mood,
                ImageUrl = eventDto.ImageUrl
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return newEvent;
        }

        public async Task<Event> GetEvent(int id)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetEvents(EventFilterDto filter)
        {
            var query = _context.Events
                .Include(e => e.Organizer)
                .Where(e => e.Status == EventStatus.Approved);

            if (filter.StartDate.HasValue)
            {
                query = query.Where(e => e.StartDateTime >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(e => e.StartDateTime <= filter.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(filter.EventType))
            {
                query = query.Where(e => e.EventType == filter.EventType);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(e => e.Title.Contains(filter.SearchTerm) ||
                                       e.Description.Contains(filter.SearchTerm) ||
                                       e.Tags.Contains(filter.SearchTerm));
            }

            if (!string.IsNullOrEmpty(filter.Mood))
            {
                query = query.Where(e => e.Mood == filter.Mood);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(e => e.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(e => e.Price <= filter.MaxPrice.Value);
            }

            query = filter.SortBy switch
            {
                "date_asc" => query.OrderBy(e => e.StartDateTime),
                "date_desc" => query.OrderByDescending(e => e.StartDateTime),
                "price_asc" => query.OrderBy(e => e.Price),
                "price_desc" => query.OrderByDescending(e => e.Price),
                _ => query.OrderBy(e => e.StartDateTime)
            };

            return await query.ToListAsync();
        }

        public async Task<Event> UpdateEvent(int eventId, EventUpdateDto eventDto, int organizerId)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);

            if (existingEvent == null)
            {
                throw new Exception("Event not found");
            }

            if (existingEvent.OrganizerId != organizerId)
            {
                throw new Exception("Only the organizer can update this event");
            }

            if (existingEvent.StartDateTime < DateTime.UtcNow.AddHours(24))
            {
                throw new Exception("Event cannot be edited less than 24 hours before start");
            }

            var hasTicketsSold = await _context.Tickets.AnyAsync(t => t.EventId == eventId && t.Status == TicketStatus.Active);

            existingEvent.Description = eventDto.Description ?? existingEvent.Description;
            existingEvent.Tags = eventDto.Tags != null ? string.Join(",", eventDto.Tags) : existingEvent.Tags;
            existingEvent.Mood = eventDto.Mood ?? existingEvent.Mood;
            existingEvent.ImageUrl = eventDto.ImageUrl ?? existingEvent.ImageUrl;

            bool needsReModeration = false;

            if (eventDto.Title != null && eventDto.Title != existingEvent.Title)
            {
                existingEvent.Title = eventDto.Title;
                needsReModeration = true;
            }

            if (eventDto.EventType != null && eventDto.EventType != existingEvent.EventType)
            {
                existingEvent.EventType = eventDto.EventType;
                needsReModeration = true;
            }

            if (eventDto.StartDateTime.HasValue && eventDto.StartDateTime != existingEvent.StartDateTime)
            {
                existingEvent.StartDateTime = eventDto.StartDateTime.Value;
                needsReModeration = true;
            }

            if (eventDto.EndDateTime.HasValue && eventDto.EndDateTime != existingEvent.EndDateTime)
            {
                existingEvent.EndDateTime = eventDto.EndDateTime.Value;
            }

            if (eventDto.Location != null && eventDto.Location != existingEvent.Location)
            {
                existingEvent.Location = eventDto.Location;
                var (latitude, longitude) = await _mapService.GetCoordinates(eventDto.Location);
                existingEvent.Latitude = latitude;
                existingEvent.Longitude = longitude;
                needsReModeration = true;
            }

            if (eventDto.Price.HasValue)
            {
                if (hasTicketsSold)
                {
                    throw new Exception("Cannot change price after tickets have been sold");
                }
                existingEvent.Price = eventDto.Price.Value;
            }

            if (eventDto.TotalTickets.HasValue)
            {
                var soldTicketsCount = await _context.Tickets
                    .CountAsync(t => t.EventId == eventId && t.Status == TicketStatus.Active);

                if (eventDto.TotalTickets.Value < soldTicketsCount)
                {
                    throw new Exception($"Cannot set total tickets less than already sold ({soldTicketsCount})");
                }

                existingEvent.TotalTickets = eventDto.TotalTickets.Value;
                existingEvent.AvailableTickets = existingEvent.TotalTickets - soldTicketsCount;
            }

            if (needsReModeration)
            {
                existingEvent.Status = EventStatus.OnModeration;
            }

            existingEvent.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return existingEvent;
        }

        public async Task<bool> DeleteEvent(int eventId, int organizerId)
        {
            var existingEvent = await _context.Events.FindAsync(eventId);

            if (existingEvent == null)
            {
                return false;
            }

            if (existingEvent.OrganizerId != organizerId)
            {
                throw new Exception("Only the organizer can delete this event");
            }

            var hasActiveTickets = await _context.Tickets.AnyAsync(t => t.EventId == eventId && t.Status == TicketStatus.Active);

            if (hasActiveTickets)
            {
                throw new Exception("Cannot delete event with active tickets");
            }

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Event>> GetOrganizerEvents(int organizerId)
        {
            return await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventType>> GetEventTypes()
        {
            return new List<EventType>
            {
                new EventType { Name = "Театральная постановка" },
                new EventType { Name = "Концерт" },
                new EventType { Name = "Музей" },
            };
        }
    }

    public class EventType
    {
        public string Name { get; set; }
    }
}