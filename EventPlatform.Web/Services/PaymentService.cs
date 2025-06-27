using EventPlatform.Data;
using EventPlatform.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EventPlatform.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePayment(int ticketId, decimal amount, string description, string returnUrl);
        Task<bool> ProcessPaymentNotification(string notification);
    }

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
                description = description,
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

            ticket.PaymentId = responseData.id;
            await _context.SaveChangesAsync();

            return responseData.confirmation.confirmation_url;
        }

        public async Task<bool> ProcessPaymentNotification(string notification)
        {
            dynamic notificationData = JsonConvert.DeserializeObject(notification);
            string eventType = notificationData.@event;
            string paymentId = notificationData.object.id;

            if (eventType != "payment.succeeded")
            {
                return false;
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.PaymentId == paymentId);
            if (ticket == null)
            {
                return false;
            }

            ticket.Status = TicketStatus.Active;
            await _context.SaveChangesAsync();


            return true;
        }
    }
}