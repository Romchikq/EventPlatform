using EventPlatform.DTOs;
using EventPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] EventFilterDto filter)
        {
            var events = await _eventService.GetEvents(filter);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _eventService.GetEvent(id);
            return eventItem != null ? Ok(eventItem) : NotFound();
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetEventTypes()
        {
            var types = await _eventService.GetEventTypes();
            return Ok(types);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventCreateDto eventDto)
        {
            var organizerId = int.Parse(User.FindFirst("sub").Value);
            var newEvent = await _eventService.CreateEvent(eventDto, organizerId);
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventUpdateDto eventDto)
        {
            var organizerId = int.Parse(User.FindFirst("sub").Value);
            var updatedEvent = await _eventService.UpdateEvent(id, eventDto, organizerId);
            return Ok(updatedEvent);
        }

        [Authorize(Roles = "Organizer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var organizerId = int.Parse(User.FindFirst("sub").Value);
            var result = await _eventService.DeleteEvent(id, organizerId);
            return result ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Organizer")]
        [HttpGet("organizer")]
        public async Task<IActionResult> GetOrganizerEvents()
        {
            var organizerId = int.Parse(User.FindFirst("sub").Value);
            var events = await _eventService.GetOrganizerEvents(organizerId);
            return Ok(events);
        }
    }
}