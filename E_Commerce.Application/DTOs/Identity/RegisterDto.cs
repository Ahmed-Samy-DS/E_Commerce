using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTOs.Identity
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        //[EmailAddress] can be written in both ways
        public string Email { get; set; } = default!;
        [Required]
        //[DataType(DataType.Password)] // FrontEnd responsible for showing it as * 
        public string Password { get; set; } = default!;
        [Required]
        public string UserName { get; set; } = default!;
        [Required]
        public string DisplayName { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}
