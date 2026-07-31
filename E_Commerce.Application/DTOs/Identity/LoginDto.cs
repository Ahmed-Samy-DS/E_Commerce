using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTOs.Identity
{
    public class LoginDto
    {
        [Required, EmailAddress]
        //[EmailAddress] can be written in both ways
        public string Email { get; set; } = default!;
        [Required]
        //[DataType(DataType.Password)] // Front en responsible for showing it as * 
        public string Password { get; set; } = default!;

    }
}
