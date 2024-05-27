using Microsoft.AspNetCore.Identity.UI.Services;
using ThanhBuoiAPI.Models;

namespace ThanhBuoiAPI.Services
{
    public interface IEmailService : IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);

        string makeBodyEmailOrder(DonHang donhang);

        string makeBodyTicketBooked(Ve ve);
    }
}
