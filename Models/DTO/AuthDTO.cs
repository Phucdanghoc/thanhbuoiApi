using System.ComponentModel.DataAnnotations.Schema;

namespace ThanhBuoiAPI.Models.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string SDT { get; set; }
        public string Ten {  get; set; }
        // Add other registration fields as needed
    }

    public class ChangePasswordDTO
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
