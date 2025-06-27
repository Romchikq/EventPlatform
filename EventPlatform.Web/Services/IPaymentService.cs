using System.Threading.Tasks;

namespace EventPlatform.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePayment(int ticketId, decimal amount, string description, string returnUrl);
        Task<bool> ProcessRefund(string paymentId);
    }
}
public interface IPaymentService
{
    Task<string> CreatePayment(int ticketId, decimal amount, string description, string returnUrl);
    Task<bool> ProcessRefund(string paymentId);
}
