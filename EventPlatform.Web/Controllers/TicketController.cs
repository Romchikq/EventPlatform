using EventPlatform.DTOs;
using EventPlatform.Models;
using EventPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController(ITicketService ticketService, IPaymentService paymentService, IQRService qrService) : ControllerBase
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IQRService _qrService = qrService;

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseTicket([FromBody] TicketPurchaseDto purchaseDto)
        {
            var userId = int.Parse(User.FindFirst("sub").Value);
            var ticket = await _ticketService.CreateTicket(purchaseDto.EventId, userId);

            var paymentUrl = await _paymentService.CreatePayment(
                ticket.Id,
                ticket.PricePaid,
                $"Билет на мероприятие {ticket.Event.Title}",
                $"{Request.Scheme}://{Request.Host}/tickets/{ticket.Id}/success");

            return Ok(new { paymentUrl, ticketId = ticket.Id });
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetTicketQRCode(int id)
        {
            var userId = int.Parse(User.FindFirst("sub").Value);
            var ticket = await _ticketService.GetUserTicket(id, userId);

            if (ticket == null || ticket.Status != TicketStatus.Active)
            {
                return NotFound();
            }

            var qrData = $"EVENT:{ticket.EventId}:TICKET:{ticket.Id}:USER:{userId}";
            var qrCode = _qrService.GenerateQRCodeBase64(qrData);

            return Ok(new
            {
                qrCode,
                ticketInfo = new
                {
                    ticket.Id,
                    ticket.TicketNumber,
                    EventTitle = ticket.Event.Title,
                    EventDate = ticket.Event.StartDateTime,
                    Location = ticket.Event.Location,
                    UserName = $"{ticket.User.FirstName} {ticket.User.LastName}"
                }
            });
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateTicket([FromBody] ValidateTicketDto validateDto)
        {
            var organizerId = int.Parse(User.FindFirst("sub").Value);
            var result = await _ticketService.ValidateTicket(validateDto.QrData, organizerId);

            if (result == null)
            {
                return BadRequest(new { message = "Invalid ticket" });
            }

            return Ok(result);
        }

        [HttpPost("{id}/refund")]
        public async Task<IActionResult> RequestRefund(int id)
        {
            var userId = int.Parse(User.FindFirst("sub").Value);
            var result = await _ticketService.RequestRefund(id, userId);

            if (!result)
            {
                return BadRequest(new { message = "Cannot refund this ticket" });
            }

            return Ok(new { message = "Refund requested successfully" });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetUserTickets()
        {
            var userId = int.Parse(User.FindFirst("sub").Value);
            var tickets = await _ticketService.GetUserTickets(userId);
            return Ok(tickets);
        }
    }
}