using EventPlatform.Data;
using EventPlatform.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EventPlatform.Web.Models
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymentService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.yookassa.ru/v3/");
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["YooKassa:ShopId"]}:{_configuration["YooKassa:SecretKey"]}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
        }

        public async Task<string> CreatePayment(int ticketId, decimal amount, string description, string returnUrl)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found");
            }

            var paymentData = new
            {
                amount = new
                {
                    value = amount.ToString("0.00"),
                    currency = "RUB"
                },
                capture = true,
                confirmation = new
                {
                    type = "redirect",
                    return_url = returnUrl
                },
                description,
                metadata = new
                {
                    ticketId
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(paymentData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("payments", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create payment");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject(responseContent);

            ticket.PaymentId = responseData.id.ToString();
            await _context.SaveChangesAsync();

            return responseData.confirmation.confirmation_url.ToString();
        }

        public class YooKassaNotification
        {
            [JsonProperty("event")]
            public string Event { get; set; }

            [JsonProperty("object")]
            public PaymentObject Object { get; set; }
        }

        public class PaymentObject
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        public async Task<bool> ProcessPaymentNotification(string notification)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<YooKassaNotification>(notification);

                if (data?.Event != "payment.succeeded" || data.Object?.Id == null)
                    return false;

                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.PaymentId == data.Object.Id);

                if (ticket == null)
                    return false;

                ticket.Status = TicketStatus.Active;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing notification: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ProcessRefund(string paymentId)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.PaymentId == paymentId);

            if (ticket == null || ticket.Status != TicketStatus.Active)
            {
                return false;
            }

            var refundData = new
            {
                payment_id = paymentId,
                amount = new
                {
                    value = ticket.PricePaid.ToString("0.00"),
                    currency = "RUB"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(refundData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("refunds", content);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            ticket.Status = TicketStatus.Refunded;
            ticket.RefundDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<bool> ProcessRefundAsync(string paymentId)
        {
            throw new NotImplementedException();
        }
    }
}