using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;

        public AuthenticationService(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            // Get User By Email

            var userResult = await _identityService.FindUserByEmailAsync(loginDto.Email);
            if (!userResult.IsSuccess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }

           // Check Password

           var passwordResult = await _identityService.CheckPasswordAsync(loginDto.Email, loginDto.Password, cancellationToken);
           if(!passwordResult.IsSuccess) 
           {
                return Result<UserDto>.Fail(userResult.Errors);
           }
           if(!passwordResult.data)
           {
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid Email Or Password"));
           }

            // Return Result + User Dto

            return new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = userResult.data.DisplayName,
                Token = "Token"
            };
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
           var userResult =  await  _identityService.CreateUserAsync(registerDto, cancellationToken);

           if(!userResult.IsSuccess)
           {
                return Result<UserDto>.Fail(userResult.Errors);
           }

            var user = userResult.data;
           return Result<UserDto>.OK(new UserDto()
           {
               Email = user.Email,
               DisplayName = user.DisplayName,
               Token = "Token"


           });
        }
    } 
}
 