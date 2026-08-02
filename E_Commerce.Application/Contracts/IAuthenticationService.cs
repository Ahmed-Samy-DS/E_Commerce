using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IAuthenticationService
    {
        // login 

        // Email + Password => Token, Email, DisplayName

        Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<AddressDto>> GetCurrentUserAddressAsync(string email, CancellationToken cancellationToken = default);
 
    }
}
