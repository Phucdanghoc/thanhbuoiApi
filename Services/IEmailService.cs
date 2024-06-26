using Microsoft.AspNetCore.Identity.UI.Services;
using ThanhBuoiAPI.Models;

namespace ThanhBuoiAPI.Services
{
    public interface IEmailService : IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);


        string makeBodyTicketBooked(List<Ve> ve);

        string makeBodyTicketCancel(Ve ve,double refund);
    }
}
